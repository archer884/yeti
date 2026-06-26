use std::{env, sync::Arc, thread, time::Duration};

use axum::{
    Json, Router,
    extract::{Query, State},
    routing::get,
};
use diesel::{
    PgConnection,
    r2d2::{self, ConnectionManager, Pool},
};
use flume::Sender;
use mimalloc::MiMalloc;
use search::{
    db,
    index::{FragmentIndex, FragmentWriter},
    model::{FragmentInfo, SearchOperation},
};
use serde::Deserialize;

mod logging;

const DATABASE_URL: &str = "DATABASE_URL";
const INDEX_DIRECTORY: &str = "INDEX_DIRECTORY";

#[global_allocator]
static GLOBAL: MiMalloc = MiMalloc;

#[tokio::main]
async fn main() {
    dotenvy::dotenv().ok();
    logging::initialize();

    let manager = r2d2::ConnectionManager::<PgConnection>::new(env::var(DATABASE_URL).unwrap());
    let pool = r2d2::Pool::builder().build(manager).unwrap();
    let (tx, rx) = flume::unbounded::<SearchOperation>();
    let index = Arc::new(FragmentIndex::new(env::var(INDEX_DIRECTORY).unwrap()).unwrap());

    let mut writer = index.writer().unwrap();

    // In theory, this results in single-threaded indexing, except that I believe the IndexWriter
    // itself manages a queue and threadpool. In any case, the actual load on this application is
    // expected to light, so it hardly matters.
    let _receiver = thread::spawn(move || {
        for op in rx {
            match op {
                SearchOperation::Commit => {
                    if let Err(e) = writer.commit() {
                        tracing::error!(error = %e, "failed to commit changes");
                    }
                }
                SearchOperation::Index(id) => {
                    if let Err(e) = update_fragment(id, &mut writer, &pool) {
                        tracing::error!(error = %e, "failed to update fragment id:{id}");
                    }
                }
                SearchOperation::Remove(id) => {
                    if let Err(e) = remove_fragment(id, &mut writer) {
                        tracing::error!(error = %e, "failed to remove fragment id:{id}");
                    }
                }
            }
        }
    });

    let commit_tx = tx.clone();
    let _commiter = thread::spawn(move || {
        loop {
            thread::sleep(Duration::from_secs(90));
            commit_tx.send(SearchOperation::Commit).unwrap();
        }
    });

    let app = Router::new()
        .route("/", get(query).post(add_remove))
        .with_state(AppState { index, sender: tx });

    let bind = env::var("BIND_ADDRESS").unwrap_or_else(|_| "127.0.0.1:8000".to_string());
    tracing::info!("search-api listening on {bind}");
    let listener = tokio::net::TcpListener::bind(&bind).await.unwrap();
    axum::serve(listener, app).await.unwrap();
}

#[derive(Clone)]
struct AppState {
    index: Arc<FragmentIndex>,
    sender: Sender<SearchOperation>,
}

#[derive(Deserialize)]
struct SearchParams {
    q: String,
    p: Option<usize>,
}

async fn query(
    State(state): State<AppState>,
    Query(params): Query<SearchParams>,
) -> Json<Vec<FragmentInfo>> {
    // Tantivy search is blocking; run it off the async worker so it can't stall the runtime.
    let index = state.index.clone();
    let results =
        tokio::task::spawn_blocking(move || index.search(&params.q, params.p.unwrap_or(0)))
            .await
            .unwrap()
            .unwrap();

    Json(results)
}

#[derive(Deserialize)]
struct AddRemoveParams {
    a: Option<i64>,
    r: Option<i64>,
}

async fn add_remove(State(state): State<AppState>, Query(params): Query<AddRemoveParams>) {
    if let Some(id) = params.r {
        let _ = state.sender.send(SearchOperation::Remove(id));
    }

    if let Some(id) = params.a {
        let _ = state.sender.send(SearchOperation::Index(id));
    }
}

fn update_fragment(
    id: i64,
    writer: &mut FragmentWriter,
    pool: &Pool<ConnectionManager<PgConnection>>,
) -> anyhow::Result<()> {
    tracing::debug!("update fragment id:{id}");
    let cx = pool.get()?;
    if let Some(fragment) = db::by_id(id, cx) {
        writer.update(&fragment)?;
    }
    writer.commit()?;
    Ok(())
}

fn remove_fragment(id: i64, writer: &mut FragmentWriter) -> anyhow::Result<()> {
    tracing::debug!("remove fragment id:{id}");
    writer.remove(id)?;
    writer.commit()?;
    Ok(())
}

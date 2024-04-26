use std::{env, thread, time::Duration};

use diesel::{
    r2d2::{self, ConnectionManager, Pool},
    PgConnection,
};
use flume::Sender;
use rocket::{get, post, routes, serde::json::Json, State};
use search::{
    db,
    index::{FragmentIndex, FragmentWriter},
    model::{FragmentInfo, SearchOperation},
};

mod logging;

const DATABASE_URL: &str = "DATABASE_URL";
const INDEX_DIRECTORY: &str = "INDEX_DIRECTORY";

#[rocket::main]
async fn main() -> Result<(), rocket::Error> {
    dotenvy::dotenv().ok();
    logging::initialize();

    let manager = r2d2::ConnectionManager::<PgConnection>::new(env::var(DATABASE_URL).unwrap());
    let pool = r2d2::Pool::builder().build(manager).unwrap();
    let (tx, rx) = flume::unbounded::<SearchOperation>();
    let index = FragmentIndex::new(env::var(INDEX_DIRECTORY).unwrap()).unwrap();

    let mut writer = index.writer().unwrap();

    // In theory, this results in single-threaded indexing, except that I believe the IndexWriter
    // itself manages a queue and threadpool. Right now, we commit after every entry, because who
    // cares, but we could in theory only commit after a given number of operations have been
    // completed, or after a given amount of time has passed. In any case, the actual load on this
    // application is expected to light, so it hardly matters.
    let _receiver = thread::spawn(move || {
        for op in rx {
            match op {
                SearchOperation::Commit => {
                    if let Err(e) = writer.commit() {
                        tracing::error!(error = ?e, "failed to commit changes");
                    }
                }
                SearchOperation::Index(id) => {
                    if let Err(e) = update_fragment(id, &mut writer, &pool) {
                        tracing::error!(error = ?e, "failed to update fragment id:{id}");
                    }
                }
                SearchOperation::Remove(id) => {
                    if let Err(e) = remove_fragment(id, &mut writer) {
                        tracing::error!(error = ?e, "failed to remove fragment id:{id}");
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

    let _rocket = rocket::build()
        .manage(tx)
        .manage(index)
        .mount("/", routes![add_remove, query])
        .launch()
        .await?;

    Ok(())
}

#[get("/?<q>&<p>")]
fn query(q: String, p: Option<usize>, index: &State<FragmentIndex>) -> Json<Vec<FragmentInfo>> {
    let results = index.search(&q, p.unwrap_or(0)).unwrap();
    Json(results)
}

#[post("/?<a>&<r>")]
fn add_remove(a: Option<i64>, r: Option<i64>, sender: &State<Sender<SearchOperation>>) {
    if let Some(id) = r {
        sender.send(SearchOperation::Remove(id)).unwrap();
    }

    if let Some(id) = a {
        sender.send(SearchOperation::Index(id)).unwrap();
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
    Ok(())
}

fn remove_fragment(id: i64, writer: &mut FragmentWriter) -> anyhow::Result<()> {
    tracing::debug!("remove fragment id:{id}");
    writer.remove(id)?;
    Ok(())
}

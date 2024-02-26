use std::{env, thread};

use diesel::{r2d2, PgConnection};
use flume::Sender;
use rocket::{get, post, routes, serde::json::Json, State};
use search::{
    db,
    index::FragmentIndex,
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
    // completed, or after a given amount of time has passed.
    let _receiver = thread::spawn(move || {
        for op in rx {
            match op {
                SearchOperation::Index(id) => {
                    tracing::debug!("update fragment id:{id}");
                    let fragment = db::by_id(id, pool.get().unwrap());
                    if let Some(fragment) = fragment {
                        writer.update(&fragment).unwrap();
                    }
                }
                SearchOperation::Remove(id) => {
                    tracing::debug!("remove fragment id:{id}");
                    writer.remove(id).unwrap();
                }
            }
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
    if let Some(id) = a {
        sender.send(SearchOperation::Index(id)).unwrap();
    }

    if let Some(id) = r {
        sender.send(SearchOperation::Remove(id)).unwrap();
    }
}

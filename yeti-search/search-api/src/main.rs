use std::{env, thread};

use diesel::{r2d2, PgConnection};
use flume::Sender;
use rayon::prelude::*;
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

    let _receiver = thread::spawn(move || {
        rx.into_iter().par_bridge().for_each(|op| match op {
            SearchOperation::Index(id) => {
                let fragment = db::by_id(id, pool.get().unwrap());
                if let Some(fragment) = fragment {
                    tracing::debug!(text = fragment.content, "would index fragment:{id}");
                }
            }
            SearchOperation::Remove(id) => {
                tracing::debug!("would remove fragment:{id}");
            }
        });
    });

    let index = FragmentIndex::new(env::var(INDEX_DIRECTORY).unwrap()).unwrap();
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

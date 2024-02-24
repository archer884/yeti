use std::{env, thread};

use diesel::{r2d2, PgConnection};
use flume::Sender;
use rayon::prelude::*;
use rocket::{post, routes, State};
use search::{db, model::SearchOperation};

mod logging;

#[rocket::main]
async fn main() -> Result<(), rocket::Error> {
    dotenvy::dotenv().ok();
    logging::initialize();

    let manager = r2d2::ConnectionManager::<PgConnection>::new(env::var("DATABASE_URL").unwrap());
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

    let _rocket = rocket::build()
        .manage(tx)
        .mount("/", routes![add_remove])
        .launch()
        .await?;

    Ok(())
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

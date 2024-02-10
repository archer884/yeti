mod logging;
mod schema;

use std::env;

use axum::{
    extract::{Path, State},
    http::StatusCode,
    response::IntoResponse,
    routing::get,
    Json, Router,
};
use deadpool_diesel::{postgres::Pool, Manager};
use diesel::{
    deserialize::Queryable,
    query_dsl::methods::{FilterDsl, LimitDsl, SelectDsl},
    result::Error::NotFound,
    ExpressionMethods, RunQueryDsl, Selectable, SelectableHelper,
};
use schema::Fragments;
use serde::Serialize;
use tokio::net::TcpListener;

#[derive(Serialize, Selectable, Queryable)]
#[diesel(table_name = Fragments)]
struct Fragment {
    #[diesel(column_name = Heading)]
    heading: Option<String>,
    #[diesel(column_name = Content)]
    content: String,
}

#[tokio::main]
async fn main() {
    const ADDRESS: &str = "127.0.0.1:5001";

    dotenvy::dotenv().ok();
    logging::initialize();

    let manager = Manager::new(
        env::var("DATABASE_URL").unwrap(),
        deadpool_diesel::Runtime::Tokio1,
    );

    let pool = Pool::builder(manager).build().unwrap();
    let app = Router::new()
        .route("/", get(root))
        .route("/:id", get(get_fragment_text))
        .fallback(bad_request)
        .with_state(pool);

    let listener = TcpListener::bind(ADDRESS).await.unwrap();

    tracing::info!("listening on http://{ADDRESS}");

    axum::serve(listener, app).await.unwrap()
}

pub async fn root() -> &'static str {
    "active"
}

pub async fn bad_request() -> (StatusCode, &'static str) {
    (StatusCode::BAD_REQUEST, "bad request")
}

// Proof of concept. Not actually useful. Actually, I'm pretty sure the application doesn't need
// any handler to have access to the connection pool like this...
async fn get_fragment_text(
    State(pool): State<Pool>,
    Path(id): Path<i64>,
) -> Result<Json<Fragment>, (StatusCode, String)> {
    use schema::Fragments::dsl;

    tracing::debug!("request for fragment:{id}");

    let time = chronograf::start();
    let cx = pool.get().await.map_err(internal_error)?;
    let fragments = cx
        .interact(move |cx| {
            schema::Fragments::table
                .filter(dsl::Id.eq(id))
                .limit(1)
                .select(Fragment::as_select())
                .load(cx)
        })
        .await
        .map_err(internal_error)?
        .map_err(internal_error)?;

    let elapsed = time.finish();
    tracing::debug!("{:?}", elapsed);

    match fragments.into_iter().next() {
        Some(fragment) => Ok(Json(fragment)),
        None => Err((StatusCode::NOT_FOUND, String::from("not found"))),
    }
}

fn internal_error<E>(err: E) -> (StatusCode, String)
where
    E: std::error::Error,
{
    (StatusCode::INTERNAL_SERVER_ERROR, err.to_string())
}

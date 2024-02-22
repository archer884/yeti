mod logging;

fn main() {
    dotenvy::dotenv().ok();
    logging::initialize();
    tracing::info!("Logging ok!");
}

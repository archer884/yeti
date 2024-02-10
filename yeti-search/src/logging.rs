use tracing::Level;

pub fn initialize() {
    tracing_subscriber::fmt()
        .with_max_level(Level::TRACE)
        .pretty()
        .init();
}

use tracing::Level;

pub fn initialize() {
    tracing_subscriber::fmt()
        .pretty()
        .with_max_level(Level::DEBUG)
        .init();
}

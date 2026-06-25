use tracing::Level;
use tracing_subscriber::{filter::Targets, layer::SubscriberExt, util::SubscriberInitExt};

pub fn initialize() {
    let target = Targets::new()
        .with_target("yeti_search", Level::DEBUG)
        .with_target("search", Level::DEBUG)
        .with_target("axum", Level::INFO)
        .with_target("hyper", Level::WARN);

    let subscriber = tracing_subscriber::fmt()
        .with_max_level(Level::DEBUG)
        .pretty()
        .finish();

    subscriber.with(target).init();
}

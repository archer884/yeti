pub mod db;
pub mod error;
pub mod index;
pub mod model;
pub mod schema;

pub type Result<T, E = error::Error> = std::result::Result<T, E>;

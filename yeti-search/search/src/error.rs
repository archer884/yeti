#[derive(Debug, thiserror::Error)]
pub enum Error {
    #[error(transparent)]
    Db(#[from] diesel::result::Error),
    #[error(transparent)]
    Tantivy(#[from] tantivy::TantivyError),
    #[error("unable to open mmap directory: {0}")]
    TantivyDirectory(#[from] tantivy::directory::error::OpenDirectoryError),
}

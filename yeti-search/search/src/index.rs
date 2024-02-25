use std::path::PathBuf;

use diesel::{connection::LoadConnection, pg::Pg};
use tantivy::{
    directory::MmapDirectory,
    doc,
    schema::{self, Field, Schema},
    Index, IndexWriter,
};

use crate::{db, model::Fragment};

const MEGABYTE: usize = 0x100000;
const DEFAULT_ARENA_SIZE: usize = 2048;

pub struct IndexBuilder {
    /// sets the path where the index will be stored
    path: PathBuf,
    /// sets the amount of memory to use for indexing
    memory: usize,
    schema: FragmentSchema,
}

impl IndexBuilder {
    pub fn new(path: impl Into<PathBuf>) -> Self {
        Self {
            path: path.into(),
            memory: DEFAULT_ARENA_SIZE * MEGABYTE,
            schema: FragmentSchema::create(),
        }
    }

    pub fn create_index<T>(&self, fragments: db::FragmentsPageIter<T>) -> crate::Result<()>
    where
        T: LoadConnection<Backend = Pg>,
    {
        // FIXME: In the event the index exists, do we want to delete it first?
        let index = Index::open_or_create(MmapDirectory::open(&self.path)?, self.schema())?;
        let mut writer = index.writer(self.memory)?;

        for page in fragments {
            let page = page?;
            self.index_page(&mut writer, &page)?;
        }

        Ok(())
    }

    fn index_page(&self, writer: &mut IndexWriter, page: &[Fragment]) -> tantivy::Result<()> {
        let schema = &self.schema;
        for fragment in page {
            writer.add_document(doc!(
                schema.id => fragment.id,
                schema.writer_id => fragment.writer_id,
                schema.manuscript_id => fragment.manuscript_id,
                schema.content => fragment.content.as_str(),
            ))?;
        }
        Ok(())
    }

    fn schema(&self) -> Schema {
        self.schema.schema.clone()
    }
}

pub struct FragmentSchema {
    id: Field,
    writer_id: Field,
    manuscript_id: Field,
    content: Field,
    schema: Schema,
}

impl FragmentSchema {
    pub fn create() -> Self {
        let mut builder = Schema::builder();
        Self {
            id: builder.add_i64_field("id", schema::STORED),
            writer_id: builder.add_i64_field("writer_id", schema::STORED),
            manuscript_id: builder.add_i64_field("manuscript_id", schema::STORED),
            content: builder.add_text_field("content", schema::TEXT),
            schema: builder.build(),
        }
    }
}

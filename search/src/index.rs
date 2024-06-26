use std::{
    fs, io,
    path::{Path, PathBuf},
};

use diesel::{connection::LoadConnection, pg::Pg};
use tantivy::{
    collector::TopDocs,
    directory::MmapDirectory,
    doc,
    query::QueryParser,
    schema::{self, Field, Schema, TextFieldIndexing, TextOptions, Value},
    TantivyDocument, Index, IndexReader, IndexWriter, Term,
};

use crate::{
    db,
    model::{Fragment, FragmentInfo},
};

const MEGABYTE: usize = 0x100000;
const DEFAULT_ARENA_SIZE: usize = 2048;
const SEARCH_PAGE_SIZE: usize = 10;

pub struct FragmentIndex {
    // FIXME: I expect to use this for writing, but we'll see.
    index: Index,
    reader: IndexReader,
    schema: FragmentSchema,
    parser: QueryParser,
}

impl FragmentIndex {
    pub fn new(path: impl AsRef<Path>) -> crate::Result<Self> {
        let path = ensure_path(path.as_ref())?;
        let dir = MmapDirectory::open(path)?;
        let schema = FragmentSchema::create();
        let index = Index::open_or_create(dir, schema.schema.clone())?;
        let reader = index.reader()?;
        let parser = QueryParser::for_index(&index, vec![schema.content]);

        Ok(Self {
            index,
            reader,
            schema,
            parser,
        })
    }

    pub fn search(&self, query: &str, page: usize) -> crate::Result<Vec<FragmentInfo>> {
        let time = chronograf::start();

        let query = self.parser.parse_query(query)?;
        let collector = TopDocs::with_limit(SEARCH_PAGE_SIZE).and_offset(page * SEARCH_PAGE_SIZE);
        let searcher = self.reader.searcher();
        let results = searcher.search(&query, &collector)?;

        let results = results
            .into_iter()
            .filter_map(|(_, addr)| searcher.doc(addr).ok())
            .map(|document| FragmentInfo {
                id: self.schema.id.get_i64(&document),
                writer_id: self.schema.writer_id.get_i64(&document),
                manuscript_id: self.schema.manuscript_id.get_i64(&document),
            })
            .collect();

        tracing::info!(elapsed = ?time.finish(), "query complete");

        Ok(results)
    }

    pub fn writer(&self) -> crate::Result<FragmentWriter> {
        let writer = self.index.writer(DEFAULT_ARENA_SIZE * MEGABYTE)?;
        Ok(FragmentWriter {
            writer,
            schema: self.schema.clone(),
            update_count: 0,
        })
    }
}

pub struct FragmentWriter {
    writer: IndexWriter,
    schema: FragmentSchema,
    update_count: u8,
}

impl FragmentWriter {
    pub fn update(&mut self, fragment: &Fragment) -> crate::Result<()> {
        self.writer.add_document(self.schema.document(fragment))?;
        self.update_count += 1;
        self.commit()?;
        Ok(())
    }

    pub fn remove(&mut self, id: i64) -> crate::Result<()> {
        let term = Term::from_field_i64(self.schema.id, id);
        self.writer.delete_term(term);
        self.writer.commit()?;
        Ok(())
    }

    pub fn commit(&mut self) -> crate::Result<()> {
        if self.update_count > 9 {
            self.writer.commit()?;
            self.update_count = 0;
        }

        Ok(())
    }
}

fn ensure_path(path: &Path) -> io::Result<&Path> {
    if !path.exists() {
        fs::create_dir_all(path)?;
    }

    Ok(path)
}

/// An index builder used by the CLI to construct the index from scratch.
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
        let index = Index::open_or_create(MmapDirectory::open(&self.path)?, self.schema())?;
        let mut writer = index.writer(self.memory)?;

        for page in fragments {
            let page = page?;
            self.index_page(&mut writer, &page)?;
        }

        Ok(())
    }

    fn index_page(&self, writer: &mut IndexWriter, page: &[Fragment]) -> tantivy::Result<()> {
        writer.delete_all_documents()?;
        for fragment in page {
            writer.add_document(self.schema.document(fragment))?;
        }
        writer.commit()?;
        Ok(())
    }

    fn schema(&self) -> Schema {
        self.schema.schema.clone()
    }
}

#[derive(Clone)]
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
        let content_options = TextOptions::default().set_indexing_options(
            TextFieldIndexing::default()
                .set_tokenizer("en_stem")
                .set_index_option(schema::IndexRecordOption::WithFreqsAndPositions),
        );

        Self {
            // id field must be indexed in order for us to delete the document on the basis of the id term
            id: builder.add_i64_field("id", schema::INDEXED | schema::STORED),
            writer_id: builder.add_i64_field("writer_id", schema::STORED),
            manuscript_id: builder.add_i64_field("manuscript_id", schema::STORED),
            content: builder.add_text_field("content", content_options),
            schema: builder.build(),
        }
    }

    pub fn document(&self, fragment: &Fragment) -> TantivyDocument {
        doc!(
            self.id => fragment.id,
            self.writer_id => fragment.writer_id,
            self.manuscript_id => fragment.manuscript_id,
            self.content => fragment.content.as_str(),
        )
    }
}

trait FieldAccess {
    fn get_i64(self, document: &TantivyDocument) -> i64;
}

impl FieldAccess for Field {
    fn get_i64(self, document: &TantivyDocument) -> i64 {
        unsafe {
            document
                .field_values()
                .get_unchecked(0)
                .value()
                .as_i64()
                .unwrap()
        }
    }
}

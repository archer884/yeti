use diesel::{Queryable, Selectable};
use serde::Serialize;

use crate::schema::Fragments;

#[derive(Clone, Copy, Debug)]
pub enum SearchOperation {
    Index(i64),
    Remove(i64),
}

#[derive(Debug, Serialize, Selectable, Queryable)]
#[diesel(table_name = Fragments)]
pub struct Fragment {
    #[diesel(column_name = Id)]
    pub id: i64,

    #[diesel(column_name = WriterId)]
    pub writer_id: i64,

    #[diesel(column_name = ManuscriptId)]
    pub manuscript_id: i64,

    #[diesel(column_name = Heading)]
    pub heading: Option<String>,

    #[diesel(column_name = Content)]
    pub content: String,
}

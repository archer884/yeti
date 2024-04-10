use diesel::{connection::LoadConnection, pg::Pg, prelude::*};

use crate::{model::Fragment, schema};

#[derive(Debug)]
pub struct FragmentsPageIter<T> {
    page: i64,
    connection: T,
}

impl<T> Iterator for FragmentsPageIter<T>
where
    T: LoadConnection<Backend = Pg>,
{
    type Item = diesel::QueryResult<Vec<Fragment>>;

    fn next(&mut self) -> Option<Self::Item> {
        let fragments = match load_page(self.page, &mut self.connection) {
            Ok(fragments) => fragments,
            Err(e) => return Some(Err(e)),
        };

        self.page += 1;
        Some(fragments).filter(|f| !f.is_empty()).map(Ok)
    }
}

pub fn load_fragments<T>(connection: T) -> FragmentsPageIter<T>
where
    T: LoadConnection<Backend = Pg>,
{
    FragmentsPageIter {
        page: 0,
        connection,
    }
}

pub fn load_page(
    page: i64,
    connection: &mut impl LoadConnection<Backend = Pg>,
) -> diesel::QueryResult<Vec<Fragment>> {
    const PAGE_SIZE: i64 = 20;

    let time = chronograf::start();
    let fragments: Vec<Fragment> = schema::Fragments::table
        .filter(schema::Fragments::SoftDelete.eq(false))
        .order_by(schema::Fragments::Created)
        .select(Fragment::as_select())
        .offset(page * PAGE_SIZE)
        .limit(PAGE_SIZE)
        .load(connection)?;
    tracing::info!(elapsed = ?time.finish(), "fragments loaded");

    Ok(fragments)
}

pub fn by_id(id: i64, mut connection: impl LoadConnection<Backend = Pg>) -> Option<Fragment> {
    let time = chronograf::start();

    let fragments: Vec<Fragment> = schema::Fragments::table
        .filter(schema::Fragments::Id.eq(id))
        .limit(1)
        .select(Fragment::as_select())
        .load(&mut connection)
        .unwrap();

    let elapsed = time.finish();
    tracing::debug!("fetch complete: {:?}", elapsed);

    single(fragments)
}

fn single<I: IntoIterator>(i: I) -> Option<I::Item> {
    i.into_iter().next()
}

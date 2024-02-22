//! Imagine that this program is intended to run weekly in order to ensure that the search index is
//! fully updated with regard to the contents of the database.
//!
//! ...Might make more sense to have this as part of the search project, if you ever stop being a
//! lazy idiot.

fn main() {
    static MESSAGE: &str =
        "At some point, make this program initialize the search index based on the database.";

    println!("{MESSAGE}");
}

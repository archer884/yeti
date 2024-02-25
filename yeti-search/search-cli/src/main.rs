use std::{env, fs, path::Path, process};

use anyhow::Ok;
use clap::Parser;
use diesel::{Connection, PgConnection};
use search::{db, index::IndexBuilder};

mod logging;

/// environment key referring to the tantivy index directory
const INDEX_DIRECTORY: &str = "INDEX_DIRECTORY";

#[derive(Debug, Parser)]
struct Args {
    #[command(subcommand)]
    command: Command,
}

#[derive(Debug, Parser)]
enum Command {
    Initialize,
    Search,
}

fn main() {
    dotenvy::dotenv().ok();
    logging::initialize();

    if let Err(e) = run(Args::parse()) {
        eprintln!("{e}");
        process::exit(1);
    }
}

fn run(args: Args) -> anyhow::Result<()> {
    let url = env::var("DATABASE_URL")?;
    let connection = PgConnection::establish(&url)?;

    match args.command {
        Command::Initialize => initialize(connection),
        Command::Search => search(),
    }
}

fn initialize(connection: PgConnection) -> anyhow::Result<()> {
    let directory = env::var(INDEX_DIRECTORY)?;

    if !Path::new(&directory).exists() {
        fs::create_dir_all(&directory)?;
    }

    let builder = IndexBuilder::new(&directory);
    builder.create_index(db::load_fragments(connection))?;
    Ok(())
}

fn search() -> anyhow::Result<()> {
    todo!("Guess we need some way to read the index, huh.")
}

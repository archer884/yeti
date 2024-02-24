use std::{env, process};

use clap::Parser;
use diesel::{Connection, PgConnection};
use search::db;

mod logging;

#[derive(Debug, Parser)]
struct Args {
    #[command(subcommand)]
    command: Command,
}

#[derive(Debug, Parser)]
enum Command {
    List,
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
        Command::List => list_fragments(connection),
    }
}

fn list_fragments(connection: PgConnection) -> anyhow::Result<()> {
    for page in db::load_fragments(connection) {
        let page = page?;
        for fragment in page {
            println!("{:?}", fragment);
        }
    }

    Ok(())
}

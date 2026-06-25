use std::process;

use argon2::{
    Argon2, PasswordHasher,
    password_hash::{SaltString, rand_core::OsRng},
};
use clap::Parser;

/// given a password, prints a salt and hash
#[derive(Debug, Parser)]
struct Args {
    password: String,
}

fn main() {
    if let Err(e) = run(Args::parse()) {
        eprintln!("{e}");
        process::exit(1);
    }
}

fn run(args: Args) -> anyhow::Result<()> {
    let salt = SaltString::generate(&mut OsRng);
    let argon = Argon2::default();
    let hash = argon.hash_password(args.password.as_bytes(), &salt)?;
    println!("{}", salt.as_salt().len());
    println!("{}", hash.hash.unwrap().as_bytes().len());
    println!("{hash}");
    Ok(())
}

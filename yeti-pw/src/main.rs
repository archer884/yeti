use std::process;

use argon2::{password_hash::{rand_core::OsRng, SaltString}, Argon2, PasswordHasher};
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
    println!("{hash}");
    Ok(())
}

// $argon2id$v=19$m=19456,t=2,p=1$iTtyFAnGAlN2PfCytMLAkw$wd4nZ0E1ujdrnn5TC+rjT4XemaoKiWM29ArBJCadpK8
// iTtyFAnGAlN2PfCytMLAkw
// wd4nZ0E1ujdrnn5TC+rjT4XemaoKiWM29ArBJCadpK8

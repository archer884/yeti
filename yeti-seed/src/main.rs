use std::{fs, io, path::Path, process};

use clap::Parser;
use reqwest::blocking::Client;
use serde::{Deserialize, Serialize};

#[derive(Debug, Parser)]
struct Args {
    /// gutenberg data path
    path: String,

    /// auth token
    token: String,
}

#[derive(Debug)]
struct YetiClient {
    client: Client,
    token: String,
}

impl YetiClient {
    fn new(token: impl Into<String>) -> Self {
        Self {
            client: Client::new(),
            token: token.into(),
        }
    }

    fn create_manuscript(&self, title: &str) -> reqwest::Result<ManuscriptSummary> {
        self.client
            .post("http://localhost:5000/manuscript")
            .bearer_auth(&self.token)
            .json(&CreateManuscript { title, blurb: None })
            .send()?
            .json()
    }

    fn add_content(
        &self,
        summary: &ManuscriptSummary,
        text: &str,
    ) -> reqwest::Result<FragmentSummary> {
        self.client
            .post("http://localhost:5000/fragment")
            .bearer_auth(&self.token)
            .json(&CreateFragment {
                manuscript_id: summary.id,
                heading: None,
                content: text,
            })
            .send()?
            .json()
    }
}

#[derive(Debug, Deserialize)]
struct TextInformation {
    id: i32,
    title: String,
}

#[derive(Debug, Serialize)]
#[serde(rename_all = "camelCase")]
struct CreateManuscript<'a> {
    title: &'a str,
    blurb: Option<&'a str>,
}

#[allow(unused)]
#[derive(Debug, Deserialize)]
#[serde(rename_all = "camelCase")]
struct ManuscriptSummary {
    id: i64,
    writer_id: i64,
    title: String,
    blurb: Option<String>,
}

#[derive(Debug, Serialize)]
#[serde(rename_all = "camelCase")]
struct CreateFragment<'a> {
    manuscript_id: i64,
    heading: Option<&'a str>,
    content: &'a str,
}

#[allow(unused)]
#[derive(Debug, Deserialize)]
#[serde(rename_all = "camelCase")]
struct FragmentSummary {
    id: i64,
    writer_id: i64,
    manuscript_id: i64,
    heading: Option<String>,
    length: i32,
    sort_by: f64,
}

fn main() {
    if let Err(e) = run(Args::parse()) {
        eprintln!("{e:#?}");
        process::exit(1);
    }
}

fn run(args: Args) -> anyhow::Result<()> {
    let path = Path::new(&args.path);
    let database = read_database(path)?;
    let files = database
        .iter()
        .map(|info| read_file(path, info).map(|text| (info, text)));

    let client = YetiClient::new(&args.token);
    for file in files {
        let (info, text) = file?;
        handle_text(&client, info, &text)?;
        println!("{} / {}", info.title, text.len());
    }

    Ok(())
}

fn handle_text(
    client: &YetiClient,
    info: &TextInformation,
    text: &str,
) -> reqwest::Result<(i64, i64)> {
    // In order to create a text on the server, we need to do two things:
    // First, create a new manuscript. The endpoint for this returns a manuscript summary.
    // Using the id found in the manuscript summary, we can then add a single fragment to
    // the manuscript containing the full text of the gutenberg document.

    let manuscript = dbg!(client.create_manuscript(&info.title)?);
    let fragment = dbg!(client.add_content(&manuscript, text)?);
    Ok((manuscript.id, fragment.id))
}

fn read_file(path: &Path, info: &TextInformation) -> io::Result<String> {
    let file_name = format!("{}-0.txt", info.id);
    let path = path.join(file_name);
    fs::read_to_string(path)
}

fn read_database(path: &Path) -> anyhow::Result<Vec<TextInformation>> {
    let text = fs::read_to_string(path.join("top100.json"))?;
    Ok(serde_json::from_str(&text)?)
}

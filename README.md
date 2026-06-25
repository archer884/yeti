# Yeti

A public library.

## State of the app

### Yeti.Api

Based on core and db, the api is intended to permit users to add and edit manuscripts for others to view. This functionality works. It currently lacks any way for users to register; the seeded login is `longfellow`. Tokens have real lifetimes now (access 15 min, refresh 30 days), and dev startup prints a 7-day access token for convenience.

### yeti-vue

A Vue 3 + Vite front end. Replaced an earlier remix front end. Still the default scaffold right now — real UI work has not started.

### search

Provides full-text indexing for manuscripts stored on yeti. The `search` library wraps Tantivy; `search-api` is an axum HTTP server the api talks to, and `search-cli` rebuilds/queries the index.

### yeti-pw

Utility to argon2-hash a password for a login record.

### yeti-seed

CLI to bulk-import Project Gutenberg texts via the api.

## Development

The database runs in Docker (see `docker-compose.yml`). For day-to-day work on the front end,
the api, or the reader site, run **only the db** container and everything else on the host:

```sh
docker compose up -d db                    # just postgres, on localhost:5432
dotnet run --project Yeti.Api              # api on 5000 (prints a 7-day dev token)
dotnet run --project Yeti.Web              # reader site on 5002
cargo run -p search-api                    # search on 8000 (only if exercising search)
cargo run -p search-cli -- initialize      # build the Tantivy index (once / as needed)
```

The host apps already target `localhost:5432` (`.env` + `appsettings.json`), and the SPA dev server
(`npm run dev` in `yeti-vue/`, needs nvm) is in the api's CORS allowed origins. Apply the schema
once with `dotnet ef database update --project Yeti.Db -- .env`; the `yeti-db` volume keeps it.

There are two modes that **share the same host ports** (5000/5002/8000/5432), so don't run both at
once:

- *dev* — `docker compose up -d db`, run the apps locally (above).
- *full stack* — `docker compose up -d --build` runs the db, a one-shot migration, the Rust
  `search-api`, and the .NET api/web in containers (api on host `5050` since macOS AirPlay holds
  `5000`). Stop the app containers to switch back to dev: `docker compose stop api web search-api`.

You can mix freely too — e.g. keep the db + search-api containerized and run only Yeti.Api/Yeti.Web
on the host for debugging.

# AGENTS.md

Reference for AI agents (and humans) working in the Yeti codebase.

## What is Yeti

Yeti is an in-progress prototype for an online fiction/publishing platform ("a public
library"). Writers create **manuscripts**, each composed of ordered **fragments** (chapters/
sections). Manuscripts can be tagged and full-text-searched. See `README.md` for the author's
own status notes.

## Polyglot monorepo

The project is a .NET backend + reader site, a Rust search service, and a Vue author SPA in one repo.

| Path | Lang | Role |
|------|------|------|
| `Yeti.Api/` | C# (.NET 10) | ASP.NET Core Web API — the JSON backend (JWT bearer) |
| `Yeti.Web/` | C# (.NET 10) | Server-rendered reader site (Razor Pages + htmx, cookie auth) |
| `Yeti.Core/` | C# (.NET 10) | Domain logic: services, providers, DTOs, config |
| `Yeti.Db/` | C# (.NET 10) | EF Core data layer (PostgreSQL) |
| `Yeti.Test/` | C# (.NET 10) | xUnit tests |
| `search/` | Rust (2024) | Tantivy full-text index library |
| `search-api/` | Rust | Rocket HTTP server wrapping `search/` |
| `search-cli/` | Rust | CLI to rebuild/query the index |
| `yeti-pw/` | Rust | Utility to argon2-hash a password |
| `yeti-seed/` | Rust | CLI to bulk-import Project Gutenberg texts via the API |
| `yeti-vue/` | TS/Vue 3 | Author SPA, built to `/author/` (Vite, base `'/author/'`) |

## Prerequisites & tooling

- `dotnet` SDK (.NET 10) — present.
- `cargo` (Rust toolchain) — present. Repo has `.cargo/config.toml` enabling
  `-Ctarget-cpu=native` and `lld` linker; release profile uses `lto`, single codegen unit,
  `panic=abort`.
- `node`/`npm` are **installed via nvm** and NOT on the default PATH. Prefix shell commands with
  `source ~/.nvm/nvm.sh &&` (node v24) when running anything in `yeti-vue/`.
- PostgreSQL. The dev DB runs in Docker via `docker compose up -d` (see `docker-compose.yml` —
  `postgres:17`, user `yetiuser`, db `yeti`, host port 5432; data in the `yeti-db` named volume).
  Connection strings live in `.env` (gitignored, for the Rust side + EF design-time factory) and in
  `Yeti.Api/appsettings.json` / `Yeti.Web/appsettings.json` (committed, runtime — both point at
  `localhost:5432`).

## Commands

### Database (Docker, run from repo root)

```sh
docker compose up -d                                                          # start postgres
docker exec yeti-db psql -U yetiuser -d yeti -c "SELECT \"Id\",\"Username\" FROM \"Writers\";"  # quick query
dotnet ef database update --project Yeti.Db -- .env                           # apply migrations
cargo run -p search-cli -- initialize                                         # rebuild the Tantivy index
```

### Full stack (Docker Compose)

Each service has a Dockerfile (`Yeti.Api/`, `Yeti.Web/`, `search-api/`, `Yeti.Db/`) and they're
wired together in `docker-compose.yml`. Services address each other by service name over the
compose network (e.g. the apps' `Search__Url` is `http://search-api:8000`, and the DB host is
`db`). Host ports: reader site `5002`, API `5050` (5000 is taken by macOS AirPlay Receiver),
search-api `8000`, postgres `5432`.

```sh
docker compose up -d --build                                                  # build + run everything
docker compose logs -f api web                                                # tail app logs
docker compose --profile tools run --rm search-cli initialize                 # rebuild the index
docker compose down                                                           # stop (named volumes persist)
```

`migrate` is a one-shot service that applies EF migrations (incl. seed) before the apps start, and
reads `CONNECTION_STRING` from the environment. `search-cli` is behind the `tools` profile (it
shares the `yeti-index` volume; stop `search-api` first since it holds the index writer lock).
The `WriterContextFactory` reads `CONNECTION_STRING` from the env first, then falls back to `.env`.

### .NET (run from repo root)

```sh
dotnet build Yeti.sln              # build the whole solution
dotnet test Yeti.sln               # run xUnit tests
dotnet run --project Yeti.Api      # run the API, port 5000 (dev: prints a 7-day token on startup)
dotnet run --project Yeti.Web      # run the reader site, port 5002
dotnet watch run --project Yeti.sln  # hot-reload build (matches .vscode/tasks.json)
```

### Rust (run from repo root — workspace defined in `Cargo.toml`)

```sh
cargo build
cargo run -p search-api                      # run the search HTTP server (port from config)
cargo run -p search-cli -- initialize        # rebuild the index from the DB
cargo run -p search-cli -- search "<query>"  # query the index from the CLI
cargo run -p yeti-pw -- "<password>"         # print an argon2id hash for a login record
cargo run -p yeti-seed -- "<gutenberg-path>" "<auth-token>"
```

`search-api` and `search-cli` read `DATABASE_URL` and `INDEX_DIRECTORY` from `.env` (via
`dotenvy`). The index directory defaults to `./.yeti-index`.

### Vue (run from `yeti-vue/`, needs nvm sourced)

```sh
npm run dev         # vite dev server (http://localhost:5173/author/)
npm run build       # type-check + production build -> Yeti.Web/wwwroot/author/
npm run type-check  # vue-tsc
npm run lint        # oxlint + eslint (with --fix)
npm run format      # prettier
```

`npm run build` emits straight into `Yeti.Web/wwwroot/author/` (vite `build.outDir`), so after a
build `dotnet run --project Yeti.Web` serves the SPA at `/author`. In dev the SPA runs on Vite and
calls `Yeti.Api` (port 5000) cross-origin — the API's CORS policy (`Cors:Origins` in
`Yeti.Api/appsettings.json`) allows the Vite and Yeti.Web origins.

## Architecture

### .NET layering

`Yeti.Db` (no internal deps) -> `Yeti.Core` (depends on Db) -> `Yeti.Api`/`Yeti.Web` (each depends
on Core + Db). `Yeti.Test` references `Yeti.Core` only.

**`Yeti.Api`** — entry point `Program.cs`. Uses **Lamar** as the DI container
(`ServiceRegistry`, registered via `ConfigureServices`). JWT bearer auth; Swagger only in
Development. The environment is driven by the `"Environment"` key in `appsettings.json` (the
code sets `ASPNETCORE_ENVIRONMENT` from it). `WriterContext` is a pooled Npgsql DbContext. A CORS
policy (`"YetiWeb"`, `Cors:Origins`) allows the author SPA and `Yeti.Web` to call it cross-origin.

- Controllers live in `Yeti.Api/Controller/` and derive from `YetiController`, which exposes
  `UserId` (parsed from the JWT `"id"` claim). Write endpoints are `[Authorize]`; read/search
  endpoints in `LoginController`, `ReadController`, `SearchController`, `TestController` are
  anonymous.
- `TokenService` (in `Yeti.Api/Service/`) generates JWTs. Defaults: access 15 min, refresh
  30 days. On dev startup the API prints a 7-day token to stdout for easy testing.
- Config helpers in `Yeti.Api/Config/`: `ConfigurationExtensions` (auth + Swagger setup),
  `TokenOptions`.

**`Yeti.Web`** — the **server-rendered reader site** (Razor Pages + htmx, port 5002). This is the
bimodal "reader" half of the frontend: anonymous visitors and crawlers get fast, SEO-friendly HTML
with no client framework. Entry point `Program.cs`; uses **Lamar** like `Yeti.Api`, **cookie**
auth (logged-in readers), and a pooled `WriterContext`. Pages live in `Yeti.Web/Pages/` and call
`Yeti.Core` services/providers directly (no HTTP hop):
- `Index` (home: recently-added/updated via `RecentService`, plus a "Your manuscripts" section for
  logged-in readers via `ManuscriptSummaryProvider.ByWriterId`), `Manuscript` (landing), `Read`
  (a fragment via `FragmentDetailProvider`), `Search` (full-text via `SearchService` → Rust
  search-api), `Tag`/`Tags` (browse by tag via `TagSearchService`), `Login`/`Logout` (cookie auth).
- htmx (`wwwroot/lib/htmx.min.js`) drives interactivity: live search (`hx-get` to a `?handler=`
  partial), and **load-more** pagination on home/search/tag lists (named handlers return shared
  partials `_ManuscriptListPage`/`_SearchResults`; the load-more button swaps its own row via
  `hx-target="closest .load-more-row"` + `hx-swap="outerHTML"`).
- Full-text search needs the Rust search-api; `Program.cs` wires `IndexOptions`/`IndexClient`
  (`AddConfigurable<IndexOptions>()` + a typed `HttpClient<IndexClient>`), and `Search:Url` lives
  in `appsettings.json`.
- The **author SPA is mounted at `/author`** via `MapFallbackToFile("/author/{*path}", ...)`,
  serving the production build of `yeti-vue` from `wwwroot/author/`.
- **Reader auth** is cookie-based (the `"Cookie"` scheme): `Login` validates via the existing
  `LoginService`/`HashProvider` and calls `HttpContext.SignInAsync` with an `"id"` claim (the
  writer id); `Logout` signs out. Forms use Razor Pages' auto antiforgery. Reader pages stay
  anonymous — the cookie only gates the home-page personalization hook (a fuller preference model
  for favorite tags/authors is a future task). Contrast with `Yeti.Api`, which uses JWT bearer.

**`Yeti.Core`** — all business logic.
- `Service/` — `ManuscriptService`, `FragmentService`, `LoginService`, `RecentService`,
  `SearchService`, `TagService`, `TagSearchService`, `IndexingService` (+ `IIndexingService`),
  `IndexClient` (HttpClient to the Rust search-api), and the static `FragmentExtensions`.
- `Provider/` — read-side helpers (`ManuscriptSummaryProvider`, `FragmentSummaryProvider`,
  `FragmentDetailProvider`) and `HashProvider` (Argon2id via the `Geralt` package).
- `Model/` — request/response DTOs, almost all `record`s (`CreateManuscript`,
  `UpdateFragment`, `ManuscriptSummary`, `Snapshot`, `LoginRequest`, `ModifyTag`, etc.).
- `Config/` — options-pattern plumbing: `IConfigurable` + a generic `Configure<T>` that binds
  options from `IConfiguration` sections (see `ConfigSections.cs` for `Search`/`PasswordHashing`).
  Register a configurable via `ServiceCollectionExtensions.AddConfigurable<T>()`.
- `Time.cs` — a testable clock with `Set`/`Unset` for forcing the current time in tests.

**`Yeti.Db`** — `WriterContext` (`DbContext`) with DbSets: `Writers`, `Logins`, `Manuscripts`,
`Fragments`, `Tags`. Two important overrides:
- `Update<T>` sets `Updated = UtcNow` on anything derived from `Tracked`.
- `Remove<T>` performs a **soft delete** (sets `SoftDelete = true`) for `ISoftDeletable`
  entities instead of hard-deleting.
`WriterContext.OnModelCreating` configures relationships, indexes, and `HasData` seed data
(the `longfellow` demo writer/login/manuscript/fragments). The seed uses fixed timestamps so the
model is deterministic (EF Core 10 rejects dynamic `HasData` values). EF migrations live in
`Yeti.Db/Migrations/`; design-time factory `WriterContextFactory` reads `CONNECTION_STRING`
from `.env` and strips surrounding quotes. Apply/update with
`dotnet ef database update --project Yeti.Db -- <path-to-.env>` (pass the repo-root `.env`
explicitly, since the factory resolves it relative to the project dir).

### Rust search

`search/` is a library built on **Tantivy** (the index) and **Diesel** (Postgres read access
for re-indexing). `schema.rs` is auto-generated by `diesel-cli` (note: DB column names are
**PascalCase**, e.g. `Id`, `WriterId`).

`search-api/` is a **Rocket** server exposing:
- `GET /?q=<query>&p=<page>` — search fragments, returns `FragmentInfo` JSON.
- `POST /?a=<id>&r=<id>` — add (`a`) and/or remove (`r`) a fragment by id.

Indexing happens on a background thread fed by an `flume` channel; a second thread commits
every 90 s. Uses `mimalloc` as the global allocator. The C# side talks to this server via
`IndexClient`; `IndexingService` fires index updates as best-effort, fire-and-forget `Task.Run`s
(failures are logged, not thrown).

### Vue frontend (author SPA)

Vue 3 + Vite 6 + Pinia + TypeScript. `@` is aliased to `./src`. Vite `base` is `'/author/'` and
`build.outDir` is `../Yeti.Web/wwwroot/author`, so `npm run build` lands the production bundle
where `Yeti.Web` serves it at `/author`. Largely the default scaffold right now — real UI work
has not started (it replaced an earlier `yeti-remix` frontend; see git history). It consumes the
`Yeti.Api` JSON endpoints with JWT bearer, in contrast to `Yeti.Web`'s server-rendered reader
pages — this is the intended **bimodal** split (SPA for authors, SSR for readers).

## Domain model & key concepts

- **Writer** has one **Login** (1:1) and many **Manuscripts** (1:N).
- **Manuscript** has many **Fragments** and many **Tags** (M:N via `ManuscriptTag`).
- **Fragment** holds the actual text (`Content`, optional `Heading`) and a `SortBy` (double)
  controlling display order. `FragmentExtensions.NextSortBy` computes `max(SortBy) + 1`.
- `Tracked` base class adds `Created`/`Updated`. `ISoftDeletable` marks entities with a
  `SoftDelete` flag (filtered out in queries, e.g. `.Where(x => !x.SoftDelete)`).
- **Fragment updates are append-only (copy-on-write):** `FragmentService.UpdateFragment`
  soft-deletes the old fragment and inserts a brand-new one, then re-indexes. Keep this
  pattern when extending fragment mutation logic.
- Passwords are stored argon2id in `Login.Serialized`. Generate a hash with `yeti-pw`.

## Conventions

- **C#**: file-scoped namespaces, `record` for DTOs, **primary constructors** on services/
  controllers, nullable reference types and implicit usings enabled. Constructor injection
  everywhere (no manual service registration for most concrete services — Lamar's
  auto-registration handles them).
- **Rust**: edition 2024; `anyhow` for CLIs, a custom `thiserror`-based `Error` in the
  `search` library. `tracing` for logging.
- **Comments**: the author writes informal, candid comments (including swearing). Match that
  tone rather than stripping it; do not add gratuitous comments to code that has none.
- DB **column names are PascalCase** (`Id`, `WriterId`, `ManuscriptId`). Preserve this in any
  new Diesel schema or EF mapping.

## Gotchas

- `node`/`npm` are **not on PATH** — source nvm before any `yeti-vue` command.
- `.env` is gitignored. It must define `CONNECTION_STRING` (for EF migrations via the C#
  design factory), `DATABASE_URL` (for the Rust side), and `INDEX_DIRECTORY`. Credentials are
  `yetiuser`/`yetipassword` (matching `docker-compose.yml`), host `localhost:5432`.
- `Yeti.Api/appsettings.json` and `Yeti.Web/appsettings.json` are committed and contain the dev DB
  password (API also holds the JWT key). The dev API prints a long-lived token to stdout on
  startup — handy for local scripting.
- The seed user is `longfellow`. `TokenService` sets real lifetimes (access 15 min, refresh
  30 days); dev startup prints a 7-day access token for convenience.
- Read/search endpoints are anonymous; mutating endpoints require a valid JWT whose `id` claim
  matches the owning `WriterId` of the target entity (services filter by `writerId`).

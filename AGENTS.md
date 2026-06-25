# AGENTS.md

Reference for AI agents (and humans) working in the Yeti codebase.

## What is Yeti

Yeti is an in-progress prototype for an online fiction/publishing platform ("a public
library"). Writers create **manuscripts**, each composed of ordered **fragments** (chapters/
sections). Manuscripts can be tagged and full-text-searched. See `README.md` for the author's
own (slightly stale) status notes.

## Polyglot monorepo

The project is a .NET backend, a Rust search service, and a Vue frontend in one repo.

| Path | Lang | Role |
|------|------|------|
| `Yeti.Api/` | C# (.NET 9) | ASP.NET Core Web API — the backend |
| `Yeti.Core/` | C# (.NET 9) | Domain logic: services, providers, DTOs, config |
| `Yeti.Db/` | C# (.NET 9) | EF Core data layer (PostgreSQL) |
| `Yeti.Test/` | C# (.NET 9) | xUnit tests |
| `search/` | Rust (2024) | Tantivy full-text index library |
| `search-api/` | Rust | Rocket HTTP server wrapping `search/` |
| `search-cli/` | Rust | CLI to rebuild/query the index |
| `yeti-pw/` | Rust | Utility to argon2-hash a password |
| `yeti-seed/` | Rust | CLI to bulk-import Project Gutenberg texts via the API |
| `yeti-vue/` | TS/Vue 3 | Frontend (currently just the Vite scaffold) |

## Prerequisites & tooling

- `dotnet` SDK (.NET 9 / SDK 10.x works) — present.
- `cargo` (Rust toolchain) — present. Repo has `.cargo/config.toml` enabling
  `-Ctarget-cpu=native` and `lld` linker; release profile uses `lto`, single codegen unit,
  `panic=abort`.
- `node`/`npm` are **installed via nvm** and NOT on the default PATH. Prefix shell commands with
  `source ~/.nvm/nvm/nvm.sh &&` (node v24) when running anything in `yeti-vue/`.
- PostgreSQL. The dev DB is remote; connection strings live in `.env` (gitignored) and in
  `Yeti.Api/appsettings.json` (committed, contains dev secrets).

## Commands

### .NET (run from repo root)

```sh
dotnet build Yeti.sln              # build the whole solution
dotnet test Yeti.sln               # run xUnit tests
dotnet run --project Yeti.Api      # run the API (dev: prints a 7-day token on startup)
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
npm run dev         # vite dev server
npm run build       # type-check + production build
npm run type-check  # vue-tsc
npm run lint        # oxlint + eslint (with --fix)
npm run format      # prettier
```

## Architecture

### .NET layering

`Yeti.Db` (no internal deps) -> `Yeti.Core` (depends on Db) -> `Yeti.Api` (depends on Core + Db).
`Yeti.Test` references `Yeti.Core` only.

**`Yeti.Api`** — entry point `Program.cs`. Uses **Lamar** as the DI container
(`ServiceRegistry`, registered via `ConfigureServices`). JWT bearer auth; Swagger only in
Development. The environment is driven by the `"Environment"` key in `appsettings.json` (the
code sets `ASPNETCORE_ENVIRONMENT` from it). `WriterContext` is a pooled Npgsql DbContext.

- Controllers live in `Yeti.Api/Controller/` and derive from `YetiController`, which exposes
  `UserId` (parsed from the JWT `"id"` claim). Write endpoints are `[Authorize]`; read/search
  endpoints in `LoginController`, `ReadController`, `SearchController`, `TestController` are
  anonymous.
- `TokenService` (in `Yeti.Api/Service/`) generates JWTs. Defaults: access 15 min, refresh
  30 days. On dev startup the API prints a 7-day token to stdout for easy testing.
- Config helpers in `Yeti.Api/Config/`: `ConfigurationExtensions` (auth + Swagger setup),
  `TokenOptions`.

**`Yeti.Core`** — all business logic.
- `Service/` — `ManuscriptService`, `FragmentService`, `LoginService`, `RecentService`,
  `SearchService`, `TagService`, `TagSearchService`, `IndexingService` (+ `IIndexingService`),
  `IndexClient` (HttpClient to the Rust search-api), and the static `FragmentExtensions`.
- `Provider/` — read-side helpers (`ManuscriptSummaryProvider`, `FragmentSummaryProvider`) and
  `HashProvider` (Argon2id via the `Geralt` package).
- `Model/` — request/response DTOs, almost all `record`s (`CreateManuscript`,
  `UpdateFragment`, `ManuscriptSummary`, `Snapshot`, `LoginRequest`, `ModifyTag`, etc.).
- `Config/` — options-pattern plumbing: `IConfigurable` + a generic `Configure<T>` that binds
  options from `IConfiguration` sections (see `ConfigSections.cs` for `Search`/`PasswordHashing`).
- `Time.cs` — a testable clock with `Set`/`Unset` for forcing the current time in tests.

**`Yeti.Db`** — `WriterContext` (`DbContext`) with DbSets: `Writers`, `Logins`, `Manuscripts`,
`Fragments`, `Tags`. Two important overrides:
- `Update<T>` sets `Updated = UtcNow` on anything derived from `Tracked`.
- `Remove<T>` performs a **soft delete** (sets `SoftDelete = true`) for `ISoftDeletable`
  entities instead of hard-deleting.
`WriterContext.OnModelCreating` configures relationships, indexes, and `HasData` seed data
(the `longfellow` demo writer/login/manuscript/fragments). EF migrations live in
`Yeti.Db/Migrations/`; design-time factory `WriterContextFactory` reads `CONNECTION_STRING`
from `.env` (pass the `.env` path or run from repo root).

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

### Vue frontend

Vue 3 + Vite 6 + Pinia + TypeScript. `@` is aliased to `./src`. Largely the default scaffold
right now — real UI work has not started (it replaced an earlier `yeti-remix` frontend; see
git history).

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
  design factory), `DATABASE_URL` (for the Rust side), and `INDEX_DIRECTORY`.
- `Yeti.Api/appsettings.json` is committed and contains the dev DB password and JWT key. The
  dev API prints a long-lived token to stdout on startup — handy for local scripting.
- The README's "login with test/test" / "tokens never expire" notes are stale; the seed user is
  `longfellow` and `TokenService` does set lifetimes. Trust the code over the README.
- Read/search endpoints are anonymous; mutating endpoints require a valid JWT whose `id` claim
  matches the owning `WriterId` of the target entity (services filter by `writerId`).

# PLAN.md

Roadmap of pending and future work for Yeti. See `AGENTS.md` for the architecture reference and
`README.md` for the author's status notes.

## Where things stand

The bimodal frontend is built and wired:

- **Reader site (`Yeti.Web`)** — server-rendered Razor Pages + htmx: home (recently-added/updated
  + "Your manuscripts" for logged-in readers), manuscript landing, fragment reader, full-text
  search, tag browse, load-more pagination, and reader (cookie) auth. Anonymous + SEO-friendly.
- **Author SPA (`yeti-vue`)** — mounted at `/author`, built straight into `Yeti.Web/wwwroot/author`.
  **Still the default scaffold** — no real UI yet.
- **Backend** — `Yeti.Api` (JWT) + `Yeti.Core` + `Yeti.Db` + Rust `search-api` all functional.
  CORS lets the SPA and reader site call the API cross-origin.

Everything builds clean (.NET 10) and the 3 tests pass. The dev DB is remote; reader pages are
proven to their handler/DB boundary.

## Near-term: the author SPA

This is the largest creative gap. The SPA scaffold needs to become a real authoring app. Backed
by the existing JSON API (JWT bearer):

- **Auth screen** — login form consuming `POST /login`, storing the JWT, redirecting to the
  dashboard. (Token lifetimes are real now: access 15 min, refresh 30 days.)
- **Manuscript CRUD** — list/create/edit/delete via `ManuscriptController`. The home-page
  "Your manuscripts" section already links here.
- **Fragment editor** — create/edit fragments. Must respect the **append-only (copy-on-write)**
  update model in `FragmentService.UpdateFragment` (soft-delete old + insert new + re-index).
  Add a fragment editor that handles `Heading`, `Content`, and `SortBy` ordering.
- **Tagging** — add/remove tags via `TagController` (`/tag/add`, `/tag/remove`).
- **Profile** — writer display name (`Writer.AuthorName`) editing (no endpoint exists yet — see
  "API gaps" below).
- **Routing** — add `vue-router` (the scaffold has none) and a layout. SPA fallback at `/author/*`
  already works in `Yeti.Web`.

## Trending / popular

The home page currently shows recently-added and recently-updated. "Popular/trending" was called
for in the original vision but **there is no view/read telemetry to derive it from**. Needs:

- A read-tracking model (e.g. a `FragmentReads` or `ManuscriptViews` table — increments on each
  read, possibly time-decayed for "trending" vs all-time "popular"). New EF migration.
- A `PopularService` / `TrendingService` in `Yeti.Core`, surfaced on the `Yeti.Web` home page.
- Decide whether reads are counted anonymously (all visitors) or only logged-in readers.

## Reader experience & personalization

- **Tag discovery** — the reader site has `/Tag/{value}` and `/Tags/{value+value}` but **no way
  to browse/discover tags**. Add a tag index page listing all tags (with counts), and link tags
  from manuscript pages.
- **Reader preference model** — Phase 3's personalization is the minimal "Your manuscripts" hook.
  The fuller vision (favorite tags, favorite authors) needs a preference model (new entity +
  migration), then a personalized "For you" feed on the home page.
- **Reader registration** — **no registration exists anywhere** (API or Web). Only the seeded
  `longfellow` writer can log in. Decide whether readers register (separate `Reader` entity vs
  reusing `Writer`) before building this.
- **Fragment reading UX** — next/previous fragment navigation (ordered by `SortBy`), author name
  display on the reader, a manuscript table of contents on the landing page.

## Auth hardening

- **Refresh tokens** — `TokenService` has a standing FIXME: access and refresh tokens are both
  JWTs with no way to differentiate them, and refresh tokens aren't persisted. Implement the
  plan sketched in the code: opaque refresh tokens stored in the DB (trackable/revocable), with
  a refresh endpoint. Relevant to both the API (SPA) and any future reader auth.
- **Reader vs author identity** — decide on the reader identity model (see registration above) so
  cookie auth (`Yeti.Web`) and bearer auth (`Yeti.Api`) align.

## Search & indexing

- **Excerpts/highlighting** — `SearchService` truncates `Content` to 280 chars server-side with no
  hit highlighting. Tantivy can produce snippets; wire them through for matched-term context.
- **Index drift / reconciliation** — `IndexingService` fires updates as best-effort fire-and-forget
  `Task.Run`s (failures logged, not retried). A missed update leaves the index stale. Add a
  reconciliation/rebuild path (the CLI `initialize` rebuilds from scratch; consider periodic or
  on-demand reconciliation).
- **`search/index.rs` cleanup** — `FragmentWriter.commit` has a redundant inner commit and the
  `FragmentIndex.writer()`/"expect to use this for writing" FIXME; tidy up after the 0.26 port.

## Smaller items / known issues

- **Load-more heuristic** — pagination uses `count == pageSize` to decide whether to show "Load
  more", so the last page can render an extra button that yields nothing. Inexpensive to live
  with; fix only if it bothers.
- **Pagination param** — reader pages use `?p=N`; ensure consistent across any new pages.
- **Deployment topology** — currently SPA/reader-site/API are cross-origin (CORS). A reverse
  proxy putting `/author/*`, reader routes, and `/api/*` on one origin would remove the CORS
  dependency in production. Optional.
- **Test coverage** — only `FragmentExtensions.NextSortBy` is tested (3 tests). The reader site
  (page models, auth flow) and core services have no coverage.

## Cross-cutting decisions to make

1. **Reader identity** — are readers a distinct entity from writers, or writers-who-also-read?
   Drives registration, preference model, and the auth-model alignment.
2. **What counts as "trending"** — recent velocity vs all-time popularity; and whether anonymous
   reads are counted.
3. **Author SPA framework appetite** — Vue is kept (least churn) but was never a deliberate
   choice. Still cheap to swap to Svelte before real UI lands.

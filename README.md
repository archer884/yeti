# Yeti

A public library.

## State of the app

### Yeti.Api

Based on core and db, the api is intended to permit users to add and edit manuscripts for others to view. This functionality works. It currently lacks any way for users to register; the seeded login is `longfellow`. Tokens have real lifetimes now (access 15 min, refresh 30 days), and dev startup prints a 7-day access token for convenience.

### yeti-vue

A Vue 3 + Vite front end. Replaced an earlier remix front end. Still the default scaffold right now — real UI work has not started.

### search

Provides full-text indexing for manuscripts stored on yeti. The `search` library wraps Tantivy; `search-api` is a Rocket HTTP server the api talks to, and `search-cli` rebuilds/queries the index.

### yeti-pw

Utility to argon2-hash a password for a login record.

### yeti-seed

CLI to bulk-import Project Gutenberg texts via the api.

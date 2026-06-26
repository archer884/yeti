FROM rust:1-bookworm AS build
WORKDIR /app

# libpq headers for building diesel's postgres feature.
RUN apt-get update && apt-get install -y --no-install-recommends libpq-dev \
    && rm -rf /var/lib/apt/lists/*

# Relax LTO/codegen-units for a faster image build; still an optimized release binary.
ENV CARGO_PROFILE_RELEASE_LTO=off \
    CARGO_PROFILE_RELEASE_CODEGEN_UNITS=8

COPY Cargo.toml Cargo.lock ./
COPY search/ search/
COPY search-api/ search-api/
COPY search-cli/ search-cli/
COPY yeti-pw/ yeti-pw/
COPY yeti-seed/ yeti-seed/
RUN cargo build --release -p search-api -p search-cli

FROM debian:bookworm-slim
RUN apt-get update && apt-get install -y --no-install-recommends libpq5 ca-certificates \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/target/release/search-api /usr/local/bin/search-api
COPY --from=build /app/target/release/search-cli /usr/local/bin/search-cli
EXPOSE 8000
ENTRYPOINT ["search-api"]

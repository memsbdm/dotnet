# .NET Starter Kit

Minimal .NET starter kit for building web APIs with a clean layered architecture.

## Stack

- .NET 10
- ASP.NET Core Minimal APIs
- Entity Framework Core
- PostgreSQL 18
- Redis and HybridCache
- Cookie authentication
- MailKit and Mailpit
- OpenTelemetry and Aspire Dashboard

## Architecture

- `Starter.Api` — HTTP endpoints and application configuration
- `Starter.Application` — use cases and abstractions
- `Starter.Domain` — entities, value objects, and domain rules
- `Starter.Infrastructure` — persistence and external implementations

Dependencies flow inward:

```text
Api → Application → Domain
Api → Infrastructure → Application
```

## Getting started

```bash
dotnet user-secrets set \
  "ConnectionStrings:Starter" \
  "Host=localhost;Port=5432;Database=starter;Username=starter;Password=starter-dev-password" \
  --project Starter.Api

docker compose up -d
dotnet restore
dotnet run --project Starter.Api
```

The PostgreSQL container uses development-only credentials declared in
`compose.yml`. The API connection string is stored outside the repository with
User Secrets. Use a dedicated secret manager in production.

In Development, the API applies pending migrations automatically at startup.

Mailpit captures development emails:

- SMTP: `localhost:1025`
- Web UI: <http://localhost:8025>

A successful login sends a notification email to the authenticated user.

The Aspire Dashboard displays local logs, traces, and metrics:

- Web UI: <http://localhost:18888>
- OTLP/gRPC: `localhost:4317`

The API exports telemetry only when `OpenTelemetry:OtlpEndpoint` is configured. A
local endpoint is provided in `appsettings.Development.json`; production can use
its own OpenTelemetry collector.

Health probes are available at:

- `/health/live` — the API process is running
- `/health/ready` — the API can connect to PostgreSQL and Redis

The authenticated `/me` response is cached for one minute with HybridCache:
15 seconds in local memory and Redis as the distributed secondary cache. Only
the public user profile (`Id` and `Email`) is cached.

Stop the development container with `docker compose down`.

In Development, a default user is created if it does not already exist:

```text
Email: mbadem@example.com
Password: secret123
```

The development user is configured in `Starter.Api/appsettings.Development.json` and is never seeded in Production.

## Quality

- `Starter.UnitTests` contains fast tests with no external dependencies.
- `Starter.IntegrationTests` uses disposable PostgreSQL containers and requires Docker.

```bash
dotnet build Starter.slnx
dotnet format Starter.slnx
dotnet format Starter.slnx --verify-no-changes
```

Warnings are treated as errors, and formatting conventions are defined in `.editorconfig`.

The API uses IP-partitioned sliding-window rate limits configured in `Starter.Api/appsettings.json`. The `/login` endpoint has a stricter policy and returns `429 Too Many Requests` with a `Retry-After` header when the limit is exceeded.

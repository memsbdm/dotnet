# .NET Starter Kit

Minimal .NET starter kit for building web APIs with a clean layered architecture.

## Stack

- .NET 10
- ASP.NET Core Minimal APIs
- Entity Framework Core
- SQLite
- Cookie authentication
- MailKit and Mailpit

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
docker compose up -d
dotnet restore
dotnet ef database update \
  --project Starter.Infrastructure \
  --startup-project Starter.Api
dotnet run --project Starter.Api
```

Mailpit captures development emails:

- SMTP: `localhost:1025`
- Web UI: <http://localhost:8025>

Stop the development container with `docker compose down`.

In Development, pending migrations are applied automatically and a default user is created if it does not already exist:

```text
Email: mbadem@example.com
Password: secret123
```

The development user is configured in `Starter.Api/appsettings.Development.json` and is never seeded in Production.

## Quality

```bash
dotnet build Starter.slnx
dotnet format Starter.slnx
dotnet format Starter.slnx --verify-no-changes
```

Warnings are treated as errors, and formatting conventions are defined in `.editorconfig`.

The API uses IP-partitioned sliding-window rate limits configured in `Starter.Api/appsettings.json`. The `/login` endpoint has a stricter policy and returns `429 Too Many Requests` with a `Retry-After` header when the limit is exceeded.

# .NET Starter Kit

Minimal .NET starter kit for building web APIs with a clean layered architecture.

## Stack

- .NET 10
- ASP.NET Core Minimal APIs
- Entity Framework Core
- SQLite
- Cookie authentication

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
dotnet restore
dotnet ef database update \
  --project Starter.Infrastructure \
  --startup-project Starter.Api
dotnet run --project Starter.Api
```

## Quality

```bash
dotnet build Starter.slnx
dotnet format Starter.slnx
dotnet format Starter.slnx --verify-no-changes
```

Warnings are treated as errors, and formatting conventions are defined in `.editorconfig`.

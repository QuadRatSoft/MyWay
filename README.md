# MyWay

MyWay is a logistics backend skeleton for two future boards: customer shipment requests and carrier availability.

This repository currently contains Milestone 0 only: project structure, dependency registration, Swagger, health checks, PostgreSQL through Docker Compose, and placeholder tests. Business entities, authorization, OpenIddict, EF models, DbSet declarations, and migrations are intentionally not implemented yet.

## Projects

- `src/MyWay.Web` - application host, configuration, middleware pipeline, Swagger, controllers mapping, health checks.
- `src/MyWay.Api` - HTTP layer with controllers and API registration.
- `src/MyWay.Core` - domain core. Empty for Milestone 0.
- `src/MyWay.Application` - use-case layer registration. No MediatR.
- `src/MyWay.EF` - future EF Core persistence layer registration. No entities, DbContext, DbSet, or migrations yet.
- `src/MyWay.Infrastructure` - future external technical integrations that are not database persistence.
- `tests/MyWay.UnitTests` - xUnit unit tests.
- `tests/MyWay.IntegrationTests` - xUnit integration tests.

## Project References

- `MyWay.Core` has no project references.
- `MyWay.Application` references `MyWay.Core`.
- `MyWay.Api` references `MyWay.Application` and `MyWay.Core`.
- `MyWay.EF` references `MyWay.Application` and `MyWay.Core`.
- `MyWay.Infrastructure` references `MyWay.Application` and `MyWay.Core`.
- `MyWay.Web` references `MyWay.Api`, `MyWay.Application`, `MyWay.Core`, `MyWay.EF`, and `MyWay.Infrastructure`.

## PostgreSQL

Start PostgreSQL:

```bash
docker-compose up -d
```

Default connection string:

```text
Host=localhost;Port=5432;Database=myway;Username=myway;Password=myway_password
```

## Run API

```bash
dotnet run --project src/MyWay.Web
```

Swagger is available in Development at:

```text
http://localhost:5169/swagger
```

## Checks

Health check:

```bash
curl http://localhost:5169/health
```

Ping endpoint:

```bash
curl http://localhost:5169/api/system/ping
```

Expected response:

```json
{
  "message": "MyWay API is running"
}
```

## Build And Test

```bash
dotnet build
dotnet test
```

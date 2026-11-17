# Event Registration

Event registration app: an ASP.NET Core (.NET 10) Web API with EF Core and PostgreSQL, and a React frontend (Vite, React Router 7, Tailwind CSS 4).

The only prerequisite is [Docker](https://docs.docker.com/get-docker/) with Docker Compose. Run all commands from the repository root.

## Run the app

```
docker compose up --build
```

| What       | URL                           |
|------------|-------------------------------|
| Frontend   | http://localhost:5173         |
| API        | http://localhost:5122/events  |
| Swagger UI | http://localhost:5122/swagger |
| PostgreSQL | `localhost:5433` (user/password/db: `eventregistration` / `devPassword` / `eventregistration`) |

Compose starts Postgres, then a one-off `migrate` container applies the EF Core migrations and seeds 20 sample events. After that it starts the API and the frontend (served by nginx). The data is kept in the `pgdata` volume.

- Stop: `docker compose down`
- Stop and reset the database: `docker compose down -v`
- Ports 5122, 5173 and 5433 must be free.

## Run the tests

The tests run in a .NET SDK container. They use the EF Core in-memory provider, so no database or running app is needed.

Unit tests (`RegistrationServiceTests`, the registration rules in isolation):

```
docker compose run --rm --build tests --filter Category=Unit
```

Integration tests (`EventsApiTests`, real HTTP requests through the API pipeline via `WebApplicationFactory`):

```
docker compose run --rm --build tests --filter Category=Integration
```

All tests:

```
docker compose run --rm --build tests
```

Tests are grouped with `[Trait("Category", "Unit" | "Integration")]` on the test class. Tag new test classes the same way.

## Notes
- `devPassword` is a development-only credential.

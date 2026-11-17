# Event Registration

Event registration app: an ASP.NET Core (.NET 10) Web API with EF Core and PostgreSQL, and a React frontend (Vite, React Router 7, Tailwind CSS 4).

Docker is used to run the app; tests run locally with `dotnet test`.

## Run the app

Prerequisite: [Docker](https://docs.docker.com/get-docker/) with Docker Compose. Run from the repository root.

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

Prerequisite: .NET 10 SDK. The tests use the EF Core in-memory provider, so no database or running app is needed. Run `dotnet test` from the `EventRegistration.Api` folder (the one containing `EventRegistration.Api.slnx`).

Unit tests (`RegistrationServiceTests`, the registration rules in isolation):

```
cd EventRegistration.Api
dotnet test --filter Category=Unit
```

Integration tests (`EventsApiTests`, real HTTP requests through the API pipeline via `WebApplicationFactory`):

```
dotnet test --filter Category=Integration
```

All tests:

```
dotnet test
```

## Notes
- `devPassword` is a development-only credential.

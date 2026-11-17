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

## Unfinished tasks

All requirements and constraints are covered.

## Decisions

- Layered backend (controllers, services, data/models) to separate HTTP handling, business logic and persistence. This gives a better structure and makes the business logic easier to test.
- Docker Compose to run the app, so a new user only needs Docker.
- PostgreSQL because I am familiar with it and its official Docker image is multi-architecture (amd64 and arm64), so it runs the same on Mac and PC.
- Tailwind CSS for the frontend, using utility classes with no custom CSS, because it is simpler.
- React Router for routing.
- Swagger UI added for API documentation and manual testing.

## Possible improvements

- Add paging to the events list, as there could be hundreds of events in the database.

## What was different between .NET and Java

Java EE (now Jakarta EE) with Spring, and .NET with ASP.NET Core, are very similar.

- **Language constructs:** both are statically typed, object-oriented languages with similar features.
- **Annotations and attributes:** Java annotations and C# attributes work the same way and are used for the same things. For example `@RestController` / `@GetMapping` in Spring correspond to `[ApiController]` / `[HttpGet]` in ASP.NET Core.
- **Dependency injection:** both use constructor injection, but components are registered differently. Spring discovers them through component scanning of annotated classes (`@Component`, `@Service`, `@Repository`), so registration is implicit. ASP.NET Core registers them explicitly in `Program.cs` (for example `AddScoped<IRegistrationService, RegistrationService>()`), where you also choose the lifetime (singleton, scoped or transient).
- **Server:** traditionally, Java EE applications are packaged as a WAR and deployed to a separate application server such as WildFly or Tomcat. Spring Boot embeds the server (Tomcat by default), and ASP.NET Core does the same with Kestrel, so both apps start as a plain process.

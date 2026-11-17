# Event Registration API

ASP.NET Core (.NET 10) Web API using EF Core with PostgreSQL.

## Prerequisites

- .NET 10 SDK
- PostgreSQL running locally (default port 5432)
- `dotnet-ef` installed globally: `dotnet tool install --global dotnet-ef`

## Setup

Run all `dotnet` commands from the solution folder (`EventRegistration.Api`, the one containing `EventRegistration.Api.slnx`). Because it holds a project subfolder, pass `--project EventRegistration.Api` to select the project.

### 1. Create the database role and database

The app connects as the `eventregistration` role, which owns the `eventregistration` database. Connect to your local Postgres server with `psql` as a superuser:

```
psql -U <superuser> -d postgres
```

Then run:

```sql
CREATE ROLE eventregistration WITH LOGIN PASSWORD 'yourpassword';
CREATE DATABASE eventregistration OWNER eventregistration;
```

The role needs no extra privileges. Owning the database is enough for EF Core to create its tables. The database must exist before you run migrations, because the `eventregistration` role can't create databases itself.

### 2. Configure the connection string

`appsettings.Development.json` contains a placeholder connection string (`Password=CHANGE_ME`).
Do not commit a real password. Store it in user secrets instead:

```
dotnet user-secrets init --project EventRegistration.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=eventregistration;Username=eventregistration;Password=yourpassword" --project EventRegistration.Api
```

User secrets override `appsettings.Development.json` in the Development environment.

### 3. Apply the database migration

The `InitialCreate` migration is already in the repository (`Migrations/`). Apply it to create the `Events` and `Registrations` tables in the `eventregistration` database:

```
dotnet ef database update --project EventRegistration.Api
```

For reference, the initial migration was created with:

```
dotnet ef migrations add InitialCreate --project EventRegistration.Api
```

After changing the models, add a new migration and apply it:

```
dotnet ef migrations add <MigrationName> --project EventRegistration.Api
dotnet ef database update --project EventRegistration.Api
```

### 4. Run the API

```
dotnet run --project EventRegistration.Api
```

## Notes

- A registration is unique per event and email (unique index on `EventId` + `Email`).
- Normalize emails to lowercase before saving, as Postgres compares text case-sensitively.

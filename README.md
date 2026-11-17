# Event Registration API

ASP.NET Core (.NET 10) Web API using EF Core with PostgreSQL.

## Prerequisites

- .NET 10 SDK
- PostgreSQL running locally (default port 5432)
- `dotnet-ef` installed globally: `dotnet tool install --global dotnet-ef`

## Setup

Run all commands from the project folder (`EventRegistration.Api/EventRegistration.Api`).

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
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=eventregistration;Username=eventregistration;Password=yourpassword"
```

User secrets override `appsettings.Development.json` in the Development environment.

### 3. Create and apply the database migration

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

This creates the `Events` and `Registrations` tables in the `eventregistration` database.

After changing the models, add a new migration and apply it:

```
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

### 4. Run the API

```
dotnet run
```

## Notes

- A registration is unique per event and email (unique index on `EventId` + `Email`).
- Normalize emails to lowercase before saving, as Postgres compares text case-sensitively.

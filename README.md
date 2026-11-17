# Event Registration API

ASP.NET Core (.NET 10) Web API using EF Core with PostgreSQL.

## Prerequisites

- .NET 10 SDK
- PostgreSQL running locally (default port 5432)
- `dotnet-ef` installed globally: `dotnet tool install --global dotnet-ef`

## Setup

Run all commands from the project folder (`EventRegistration.Api/EventRegistration.Api`).

### 1. Configure the connection string

`appsettings.Development.json` contains a placeholder connection string (`Password=CHANGE_ME`).
Do not commit a real password. Store it in user secrets instead:

```
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=eventregistration;Username=postgres;Password=yourpassword"
```

User secrets override `appsettings.Development.json` in the Development environment.

### 2. Create and apply the database migration

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

This creates the `eventregistration` database (if missing) with the `Events` and `Registrations` tables.

After changing the models, add a new migration and apply it:

```
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

### 3. Run the API

```
dotnet run
```

## Notes

- A registration is unique per event and email (unique index on `EventId` + `Email`).
- Normalize emails to lowercase before saving, as Postgres compares text case-sensitively.

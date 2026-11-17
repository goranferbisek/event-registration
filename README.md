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

The migrations are already in the repository (`Migrations/`). Applying them creates the `Events` and `Registrations` tables in the `eventregistration` database and seeds 20 sample programming and AI events across Slovenia (capacity 10 to 150):

```
dotnet ef database update --project EventRegistration.Api
```

The migrations are:

- `InitialCreate`: creates the schema.
- `SeedEvents`: inserts the sample events with SQL. Venues and addresses are illustrative. Rolling the migration back (`dotnet ef database update InitialCreate --project EventRegistration.Api`) deletes those events, and any registrations for them (cascade delete).

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

## Frontend

The React frontend lives in `frontend/` (Vite, React 19, React Router 7, Tailwind CSS 4).

Prerequisites: Node.js 20.19+ (or 22.12+).

```
cd frontend
npm install
npm run dev
```

The dev server runs at http://localhost:5173.

- Start the API with the `http` profile so it listens on http://localhost:5122: `dotnet run --project EventRegistration.Api --launch-profile http` (from the solution folder).
- The API base URL comes from `VITE_API_URL` in `frontend/.env.development` (default `http://localhost:5122`). `frontend/.env.example` documents the variable.
- CORS: the API allows the origin `http://localhost:5173` through `Cors:AllowedOrigins` in `appsettings.Development.json`. Add the origin there if you run the frontend on another port.

## Notes

- A registration is unique per event and email (unique index on `EventId` + `Email`).
- Normalize emails to lowercase before saving, as Postgres compares text case-sensitively.

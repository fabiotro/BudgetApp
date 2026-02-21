# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

BudgetApp is a .NET 8 ASP.NET Core MVC web application for template-based budget management, designed for events like youth camps or house camps. Users select a template (e.g., House Camp, Tent Camp) to auto-generate a budget draft, adjust values, and save finalized budgets. All amounts and totals are calculated server-side.

Data model diagram: https://dbdiagram.io/d/BudgetApp-69172fff6735e11170d8cafe

## Commands

```bash
dotnet build          # Build the project
dotnet run            # Run on http://localhost:5272
dotnet watch run      # Run with hot reload
```

### Database (Docker)

```bash
docker compose up -d          # Start SQL Server (creates DB + schema on first run)
docker compose down           # Stop container (data volume persists)
docker compose down -v        # Stop and delete all data
docker compose logs sqlserver # View SQL Server logs / init output
```

SA password: `BudgetApp_Dev@2024` — connection string is in `appsettings.Development.json`.
The init script (`docker/init-db.sh` + `docker/init-db.sql`) is idempotent and runs on every startup.

No automated test suite is configured.

## Architecture

**Pattern:** ASP.NET Core MVC + Repository Pattern

**Layers:**
- **Controllers** (`Controllers/`) — Business logic and request handling. Error handling belongs here, not in repositories.
- **Views** (`Views/`) — Razor views with Tag Helpers. Toast notifications via `_ToastNotification.cshtml` partial.
- **Models** (`Models/`) — Two kinds: `*Model` (maps directly to DB tables for Dapper) and `*ViewModel` (controller ↔ view data transfer).
- **Repositories** (`Data/Repositories/`) — One repository per table, all async, injected via interfaces. Base class `BaseRepository` shared across all.
- **Data** (`Data/DapperContext.cs`) — SQL Server connection management.

**Key files:**
- `Program.cs` — DI registration, NLog setup, localization (de-CH).
- `Extensions/TempDataExtensions.cs` — JSON serialization helpers for passing objects via TempData (used for toast notifications).
- `Enums/ToastType.cs` — Toast notification types (Success, Error, Info, Warning).
- `Resources/DataAnnotations.resx` — Localized validation messages (Swiss German, de-CH).

## Coding Conventions

Full conventions are in `CodingConventions.md`. Key points:

### Models
- **Database models**: Named `{TableName}Model` (e.g., `CampModel`). Properties must exactly match DB column names for Dapper auto-mapping. Always use DataAnnotations (`[Required]`, `[Range]`, `[StringLength]`, `[DataType]`).
- **View models**: Named `{Name}ViewModel`. Use `[Display(Name = "...")]` with localized names from `DataAnnotations.resx`.
- Use `required` modifier on non-nullable string properties.

### Repositories
- Each repository implements an interface (`I{Name}Repository<T>`), is registered as `Scoped` in DI, and implements at minimum: `Create`, `GetAll`, `GetById`, `Update`, `Delete` — all async.

### Database
- Tables: singular PascalCase (`Camp`, `Budget`, `PositionType`).
- Primary key: `Id`. Foreign keys: `{TableName}Id`.
- Column suffixes: `_fc` = forecasted, `_rl` = real data. Prefix `js_` = J+S (Jugend und Sport) data.
- Every table has `CreateDate` (set by `GETDATE()` default) and `ChangeDate` (updated by DB trigger) — never set by application code.

### Localization
- Culture is fixed to `de-CH`. All validation messages must be localized via `DataAnnotations.resx`.

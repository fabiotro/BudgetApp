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

## Formatting

C# code is formatted with CSharpier.

Before committing changes, run:

dotnet csharpier format .

Never commit C# code that fails:

dotnet csharpier check .

<!-- gitnexus:start -->
# GitNexus — Code Intelligence

This project is indexed by GitNexus as **BudgetApp** (1076 symbols, 2214 relationships, 41 execution flows).

> Index stale? Run `node .gitnexus/run.cjs analyze --index-only` from the project root — it auto-selects an available runner. No `.gitnexus/run.cjs` yet? Bootstrap with `npx`, `bunx`, or `pnpm dlx` — e.g. `bunx gitnexus@latest analyze` (npm 11 npx crash; #1939).

## Always Do

- **MUST run impact analysis before editing.** Use `impact({target: "symbolName", direction: "upstream"})` (MCP) or `node .gitnexus/run.cjs impact "symbolName" --direction upstream --repo .` (CLI fallback); report callers, processes, and risk. Never substitute grep for graph analysis.
- **MUST analyze graph changes before committing.** Use `detect_changes({scope: "all"})` (MCP) or `node .gitnexus/run.cjs detect-changes --scope all --repo .` (CLI fallback). `partial: true` or `truncated: true` is not a clean check — a zero means unseen, not unaffected; re-run it. For regression review: `detect_changes({scope: "compare", base_ref: "dev"})` or `node .gitnexus/run.cjs detect-changes --scope compare --base-ref "dev" --repo .`.
- **MUST warn the user** if impact analysis returns HIGH or CRITICAL risk before proceeding with edits.
- **MUST treat `risk: UNKNOWN` as unresolved, not as low.** An empty caller set is not evidence the symbol is unused — it can also mean the callers are not resolvable by the index (plain-object property access, dynamic dispatch, cross-language calls). `impact` pairs `UNKNOWN` with a `riskNote` saying so. Confirm with a text search before treating the symbol as safe to change or delete; do not proceed on the strength of a zero.
- When exploring unfamiliar code, use `query({search_query: "concept"})` to find execution flows instead of grepping. It returns process-grouped results ranked by relevance.
- When you need full context on a specific symbol — callers, callees, which execution flows it participates in — use `context({name: "symbolName"})`.
- For security review, `explain({target: "fileOrSymbol"})` lists taint findings (source→sink flows; needs `analyze --pdg`).

## Never Do

- NEVER edit a function, class, or method before MCP/CLI impact analysis.
- NEVER ignore HIGH or CRITICAL risk warnings from impact analysis, and never read `UNKNOWN` as an all-clear — it means the walk could not answer, which is the one verdict that requires confirming by other means.
- NEVER rename symbols with find-and-replace — use `rename` which understands the call graph.
- NEVER commit before MCP/CLI graph change analysis.

## Resources

| Resource | Use for |
| --- | --- |
| `gitnexus://repo/BudgetApp/context` | Codebase overview, check index freshness |
| `gitnexus://repo/BudgetApp/clusters` | All functional areas |
| `gitnexus://repo/BudgetApp/processes` | All execution flows |
| `gitnexus://repo/BudgetApp/process/{name}` | Step-by-step execution trace |

## CLI

| Task | Read this skill file |
| --- | --- |
| Understand architecture / "How does X work?" | `.claude/skills/gitnexus-exploring/SKILL.md` |
| Blast radius / "What breaks if I change X?" | `.claude/skills/gitnexus-impact-analysis/SKILL.md` |
| Trace bugs / "Why is X failing?" | `.claude/skills/gitnexus-debugging/SKILL.md` |
| Rename / extract / split / refactor | `.claude/skills/gitnexus-refactoring/SKILL.md` |
| Tools, resources, schema reference | `.claude/skills/gitnexus-guide/SKILL.md` |
| Index, status, clean, wiki CLI commands | `.claude/skills/gitnexus-cli/SKILL.md` |

<!-- gitnexus:end -->

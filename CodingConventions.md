# Coding Conventions for BudgetApp 

This document outlines the coding conventions to be followed when developing the Budget App. Adhering to these conventions will ensure code consistency, readability, and maintainability across the project.

*Last updated: 22.11.2025*

# Database Conventions
### Table Names
- Example table names:
	- `Camp`
	- `Budget`
	- `PositionType`
- Use singular nouns.
- Use PascalCase.
- Use English for all table names and attributes.

### Attribute Names
- Example attribute names:
	- `CampId`
	- `SubCategoryId`
	- `LeadersTeamCount_rl`
	- `js_PersonCount_fc`
- Primary keys are named with `Id`.
- The `Id` suffix is used for foreign keys. (e.g. `TemplateBudgetId`)
- Prefixes and suffixes are written in lowercase and separated with an underscore.
- Prefix descriptions:
	- `js_` => J+S (Jugend und Sport)
- Suffix descriptions:
	- `_fc` => forecasted data
	- `_rl` => real data
- Use PascalCase.
- Use singular nouns.

### Standard Attributes
Every table must have the following standard attributes:
- `Id` (int, primary key, auto-incremented): Identifier for each record.
- `CreateDate` (DateTime, nullable): Timestamp of when the record was created.
- `ChangeDate` (DateTime, nullable): Timestamp of the last update to the record.
- TODO: `CreateBy`, `ChangeBy`

The `CreateDate` is set with the default constraint `GETDATE()` in the database and the `ChangeDate` is updated with a trigger on every update.
These values are never set by the application.


# MVC

## M - Models

### Database Models
Database models are used to transfer data between the application and the database.

- Use the database naming conventions for model classes and properties.
- Extend the table names with the `Model` suffix. (e.g. `CampModel`, `BudgetModel`, `PositionModel`)
- Always name properties according to the database attribute names. => Allows automatic mapping with Dapper.
- Use `DataAnnotations` for every model property.
	- Use `[Required]` for non-nullable fields.
	- Use `[Range()]` for numeric ranges.
	- Match `[StringLength()]` with database constraints.
	- Use `[DataType(DataType.DataTypeName)]` for data that requires extra validation (e.g. Email, Phone, Password).
- Always match data types with the database.
- Add the `required` modifier to non-nullable string properties to avoid nullable warnings.

### View Models 
View models are used to transfer data between the controller and the view.
- Use the `ViewModel` suffix for view model classes. (e.g. `CampViewModel`, `BudgetViewModel`, `PositionViewModel`)
- Use the same property names as in the database models, but adapt them to the view's needs.
- Use `DataAnnotations` for validation.
	- Use `[Required]` for mandatory fields.
	- Use `[StringLength()]` to limit string lengths.
	- Use `[Range()]` for numeric ranges.
	- Use `[Display(Name = "Display Name")]` for user-friendly names. The display name is shown in the UI.
	- Use `[DataType(DataType.DataTypeName)]` for data that requires extra validation (e.g. Email, Phone, Password).
- Add the `required` modifier to non-nullable string properties to avoid nullable warnings.

## V - View

## C - Controller

# Data Access
The database is accessed using the micro-ORM `Dapper`. (https://www.learndapper.com/)

All data access is done asynchronously.
For each table, a repository class is created to handle all database operations related to that table.
These repositories are then injected into the services that require database access.
Each repository has at least the CRUD operations implemented.

- Create => `Create(Model model)`
- Read => `GetAll()`, `GetById(int id)`
- Update => `Update(Model model)`
- Delete => `Delete(int id)`

The error handling is done in the controllers and not in the repositories.
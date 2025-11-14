# BudgetApp

BudgetApp is a lightweight .NET 8 web application built with ASP.NET Core MVC and Razor Views. It focuses on creating and managing budgets derived from predefined templates, designed for scenarios such as youth camps, house camps, or other event-based budgeting. Users can choose a template (e.g., *House Camp*, *Tent Camp*), automatically generate a budget draft including all template items, adjust values, and save the finalized budget.

The presentation layer uses classical MVC controllers and Razor views with Tag Helpers. The `BudgetController` handles the main workflow: listing existing budgets, generating new budgets from templates, saving edited budgets, and displaying detailed views including calculated totals.

Data access is implemented using Dapper for high-performance, SQL-first database interaction. The repository layer loads templates and budgets, creates new budgets within a transaction, and ensures all item amounts and total sums are calculated server-side.

The data model can be viewed here:
[https://dbdiagram.io/d/BudgetApp-69172fff6735e11170d8cafe](https://dbdiagram.io/d/BudgetApp-69172fff6735e11170d8cafe)

Overall, BudgetApp provides a clean, maintainable foundation for template-based budget management with a simple UI, server-side calculations, and a clear separation of concerns across models, repositories, controllers, and views.

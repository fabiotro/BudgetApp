using BudgetApp.Data;
using BudgetApp.Data.Repositories;
using BudgetApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<IBudgetRepository<BudgetModel>, BudgetRepository<BudgetModel>>();
builder.Services.AddScoped<ICampRepository<CampModel>, CampRepository<CampModel>>();
builder.Services.AddScoped<ICategoryRepository<CategoryModel>, CategoryRepository<CategoryModel>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

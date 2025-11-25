using BudgetApp.Data;
using BudgetApp.Data.Repositories;
using BudgetApp.Models;
using NLog;
using NLog.Web;

// Early init of NLog to allow startup and exception logging, before host is built
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    builder.Services.AddScoped<DapperContext>();
    builder.Services.AddScoped<IBudgetRepository<BudgetModel>, BudgetRepository<BudgetModel>>();
    builder.Services.AddScoped<ICampRepository<CampModel>, CampRepository<CampModel>>();
    builder.Services.AddScoped<ICategoryRepository<CategoryModel>, CategoryRepository<CategoryModel>>();
    builder.Services.AddScoped<IPositionRepository<PositionModel>, PositionRepository<PositionModel>>();
    builder.Services.AddScoped<IPositionTypeRepository<PositionTypeModel>, PositionTypeRepository<PositionTypeModel>>();
    builder.Services.AddScoped<ISubCategoryRepository<SubCategoryModel>, SubCategoryRepository<SubCategoryModel>>();
    builder.Services.AddScoped<ITemplateBudgetRepository<TemplateBudgetModel>, TemplateBudgetRepository<TemplateBudgetModel>>();
    builder.Services.AddScoped<ITemplatePositionRepository<TemplatePositionModel>, TemplatePositionRepository<TemplatePositionModel>>();

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit
    LogManager.Shutdown();
}


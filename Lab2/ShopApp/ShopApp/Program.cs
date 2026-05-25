using Microsoft.EntityFrameworkCore;
using ShopApp.Components;
using ShopApp.Data;
using ShopApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
    .ClearProviders()
    .AddConsole()
    .AddDebug()
    .SetMinimumLevel(LogLevel.Information);

builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("Shop")));

builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IGoodService, GoodService>();
builder.Services.AddScoped<ISaleService, SaleService>();

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var log = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        log.LogInformation("Applying database migrations…");
        await db.Database.MigrateAsync();
        log.LogInformation("Migrations applied successfully.");
    }
    catch (Exception ex)
    {
        log.LogError(ex, "Migration failed.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

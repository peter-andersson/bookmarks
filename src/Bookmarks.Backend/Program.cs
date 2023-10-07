using Bookmarks;
using Bookmarks.Data;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("Application startup");

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();    
    
    builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
    {
        options.SerializerOptions.AddContext<SerializerContext>();
    });

    builder.Services.AddOptions<BookmarkOptions>()
        .Bind(builder.Configuration.GetSection(BookmarkOptions.SectionName))
        .ValidateDataAnnotations()
        .ValidateOnStart();

    builder.Services.AddTransient<DatabaseUpdate>();
    builder.Services.AddSingleton<Database>();
    builder.Services.AddSingleton<WebSiteInfo>();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var dbUpdate = scope.ServiceProvider.GetRequiredService<DatabaseUpdate>();
        await dbUpdate.EnsureDatabaseVersion();
    }

    app.UseStaticFiles();

    // Setup API
    BookmarkApi.MapEndpoints(app);

    app.MapFallbackToFile("index.html");

    app.Run();
}
catch (Exception exception)
{
    logger.Fatal(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}
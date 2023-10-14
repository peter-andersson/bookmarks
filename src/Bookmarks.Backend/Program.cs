using Bookmarks;
using Bookmarks.Data;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("Application startup");

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    
    builder.Services.AddDbContext<BookmarkContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Bookmark")));    
    
    builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
    {
        options.SerializerOptions.AddContext<SerializerContext>();
    });
    
    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<BookmarkContext>();
        db.Database.Migrate();        
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
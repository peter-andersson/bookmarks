using Bookmarks;
using Bookmarks.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");
    
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());
    
    builder.Services.AddDbContext<BookmarkContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Bookmark")));    
    
    builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
    {
        options.SerializerOptions.AddContext<SerializerContext>();
    });
    
    var app = builder.Build();
    
    app.UseSerilogRequestLogging();

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
    Log.Fatal(exception, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
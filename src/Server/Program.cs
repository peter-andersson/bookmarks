using System.Text.Json;
using Bookmarks.Server;
using Bookmarks.Server.Data;
using Bookmarks.Shared;
using Microsoft.AspNetCore.Identity;
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

    builder.Services.AddDbContext<BookmarkContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Bookmark")));

    builder.Services.AddControllers().AddJsonOptions(
        static options =>
            options.JsonSerializerOptions.TypeInfoResolverChain.Add(SerializerContext.Default));

    builder.Services.AddAuthorization();

    builder.Services.AddIdentityApiEndpoints<IdentityUser>()
        .AddEntityFrameworkStores<BookmarkContext>();
    
    builder.Host.UseSerilog();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<BookmarkContext>();
        db.Database.Migrate();
    }

    app.MapIdentityApi<IdentityUser>();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
    }

    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    app.UseRouting();

    BookmarkApi.MapEndpoints(app);

    app.MapControllers();
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
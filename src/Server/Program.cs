using System.Security.Claims;
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
    
    builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();
    builder.Services.AddAuthorizationBuilder();    

    builder.Services.AddDbContext<BookmarkContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Bookmark")));

    builder.Services.AddControllers().AddJsonOptions(
        static options =>
            options.JsonSerializerOptions.TypeInfoResolverChain.Add(SerializerContext.Default));
    
    builder.Services.AddIdentityCore<IdentityUser>()
        .AddEntityFrameworkStores<BookmarkContext>()
        .AddApiEndpoints();
    
    builder.Host.UseSerilog();
    
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();        

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<BookmarkContext>();
        db.Database.Migrate();
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    BookmarkApi.MapEndpoints(app);
    app.MapIdentityApi<IdentityUser>();    
    
    // provide an end point to clear the cookie for logout
    // NOTE: This logout code will be updated shortly.
    //       https://github.com/dotnet/blazor-samples/issues/132
    app.MapPost("/Logout", async (ClaimsPrincipal user, SignInManager<IdentityUser> signInManager) =>
    {
        await signInManager.SignOutAsync();
        return TypedResults.Ok();
    });

    app.UseCors("wasm");    

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
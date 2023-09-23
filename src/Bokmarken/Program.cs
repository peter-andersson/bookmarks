using Bokmarken.Database;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<DatabaseUpdate>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbUpdate = scope.ServiceProvider.GetRequiredService<DatabaseUpdate>();
    await dbUpdate.EnsureDatabaseVersion();
}

app.UseStaticFiles();

//  TODO: Implement bookmarks API
app.MapGet("/api/bookmark", () => "Get all bookmarks");
app.MapGet("/api/bookmark/{id}", (int id) => "Get one bookmark");
app.MapGet("/api/bookmark/info/{url}", (string url) => "Load info from page (title, description)");
app.MapPost("/api/bookmark", () => "Add bookmark");
app.MapPut("/api/bookmark", () => "Update bookmark");
app.MapDelete("/api/bookmark/{id}", (int id) => "Delete bookmark");

app.MapFallbackToFile("index.html");

app.Run();
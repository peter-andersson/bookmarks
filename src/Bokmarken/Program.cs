var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/api/bookmark", () => "Get all bookmarks");
app.MapGet("/api/bookmark/{id}", (int id) => "Get one bookmark");
app.MapGet("/api/bookmark/info/{url}", (string url) => "Load info from page (title, description)");
app.MapPost("/api/bookmark", () => "Add bookmark");
app.MapPut("/api/bookmark", () => "Update bookmark");
app.MapDelete("/api/bookmark/{id}", (int id) => "Delete bookmark");

app.MapFallbackToFile("index.html");

app.Run();
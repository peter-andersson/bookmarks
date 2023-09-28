using Bokmarken;
using Bokmarken.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.AddContext<SerializerContext>();
});

builder.Services.AddTransient<DatabaseUpdate>();
builder.Services.AddSingleton<Database>();
builder.Services.AddSingleton<BookmarkManager>();
builder.Services.AddSingleton<TagManager>();
builder.Services.AddSingleton<WebSiteInfo>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbUpdate = scope.ServiceProvider.GetRequiredService<DatabaseUpdate>();
    await dbUpdate.EnsureDatabaseVersion();
}

app.UseStaticFiles();

app.MapGet("/api/bookmark", async (BookmarkManager bookmarkManager) => await bookmarkManager.GetBookmarks());
app.MapGet("/api/bookmark/{id:int}", async (int id, BookmarkManager bookmarkManager) => await bookmarkManager.GetBookmark(id));
app.MapPost("/api/bookmark", async (HttpRequest request, BookmarkManager bookmarkManager) => await bookmarkManager.AddBookmark(request));
app.MapPut("/api/bookmark", async (HttpRequest request, BookmarkManager bookmarkManager) => await bookmarkManager.UpdateBookmark(request));
app.MapDelete("/api/bookmark/{id:int}", async (int id, BookmarkManager bookmarkManager) => await bookmarkManager.DeleteBookmark(id));
app.MapGet("/api/tag", async (TagManager tagManager) => await tagManager.GetTags());

app.MapPost("/api/bookmark/info",
    async (HttpRequest request, WebSiteInfo webSiteInfo) => await webSiteInfo.LoadInfo(request));

app.MapFallbackToFile("index.html");

app.Run();
using HtmlAgilityPack;

namespace Bookmarks;

public class WebSiteInfo
{
    public async Task<IResult> LoadInfo(HttpRequest request)
    {
        using var sr = new StreamReader(request.Body);
        var url = await sr.ReadToEndAsync();
    
        if (string.IsNullOrWhiteSpace(url))
        {
            return Results.BadRequest("Missing url");
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            return Results.BadRequest("Not a valid url");
        }

        var web = new HtmlWeb();
        var doc = await web.LoadFromWebAsync(url);

        var title = string.Empty;
        var titleNode = doc.DocumentNode.SelectSingleNode("//head/title");
        if (titleNode is not null)
        {
            title = titleNode.InnerText;
        }

        var description = string.Empty;
        var descriptionNode = doc.DocumentNode.SelectSingleNode("//head/meta[@name='description']");
        if (descriptionNode is not null)
        {
            description = System.Net.WebUtility.HtmlDecode(descriptionNode.GetAttributeValue("content", string.Empty));
        }

        return Results.Json(new Website(title, description));
    }
}
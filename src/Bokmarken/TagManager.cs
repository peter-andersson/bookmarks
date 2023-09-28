using Bokmarken.Data;

namespace Bokmarken;

public class TagManager
{
    private readonly ILogger<TagManager> _logger;
    private readonly Database _database;
    
    public TagManager(ILogger<TagManager> logger, Database database)
    {
        _logger = logger;
        _database = database;
    }   
    
    public async Task<IResult> GetTags()
    {
        _logger.LogInformation("Get all tags from database");

        var tags = await _database.GetTags();

        return Results.Json(tags);
    }
}
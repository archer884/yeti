using Microsoft.AspNetCore.Mvc;

using Yeti.Core.Model;
using Yeti.Core.Service;

namespace Yeti.Api.Controller;

[ApiController]
[Route("[controller]")]
public class SearchController(SearchService textSearch, TagSearchService tagSearch) : ControllerBase
{
    [HttpGet("/tag/{value}")]
    public async Task<ActionResult<List<ManuscriptSummary>>> GetByTags(string value, int page = 0)
        => await tagSearch.ByTag(value, page);

    [HttpGet("/search/{query}")]
    public async Task<ActionResult<List<Snapshot>>> GetBySearch(string query, int? page = null)
        => await textSearch.Query(query, page);
}

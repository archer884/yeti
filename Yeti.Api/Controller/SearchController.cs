using Microsoft.AspNetCore.Mvc;

using Yeti.Core.Model;
using Yeti.Core.Service;

namespace Yeti.Api.Controller;

[Route("[controller]")]
[ApiController]
public class SearchController(TagSearchService tagSearch) : ControllerBase
{
    [HttpGet("/tag/{value}/{page}")]
    public async Task<ActionResult<List<ManuscriptSummary>>> Get(string value, int page = 0)
        => await tagSearch.ByTag(value, page);
}

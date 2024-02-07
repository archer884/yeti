using Microsoft.AspNetCore.Mvc;

using Yeti.Core.Model;
using Yeti.Core.Provider;
using Yeti.Db;
using Yeti.Db.Model;

namespace Yeti.Api.Controller;

[Route("[controller]")]
[ApiController]
public class ManuscriptController(
    FragmentSummaryProvider summaryProvider,
    WriterContext context) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<Manuscript?>> Get(long id)
    {
        return await context.Manuscripts.FindAsync(id);
    }

    [HttpGet("fragments/{id}")]
    public async Task<ActionResult<List<FragmentSummary>>> GetFragments(long id)
    {
        return await summaryProvider.ByManuscriptId(id);
    }
}

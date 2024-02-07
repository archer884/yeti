using Microsoft.AspNetCore.Mvc;

using Yeti.Core.Model;
using Yeti.Core.Service;

namespace Yeti.Api.Controller;

[Route("[controller]")]
[ApiController]
public class ManuscriptController(ManuscriptService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ManuscriptSummary?>> Post([FromBody] CreateManuscript create)
    {
        return await service.CreateManuscript(create);
    }

    [HttpPut]
    public async Task<ActionResult<ManuscriptSummary?>> Update([FromBody] UpdateManuscript update)
    {
        return await service.UpdateManuscript(update);
    }

    [HttpDelete]
    public async Task<ActionResult<ManuscriptSummary?>> Delete([FromBody] DeleteManuscript delete)
    {
        return await service.DeleteManuscript(delete);
    }

    /// <summary>
    /// Retrieves a manuscript summary.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ManuscriptSummary?>> Get(long id)
    {
        return await service.ManuscriptSummary(id);
    }

    /// <summary>
    /// Retrieves a summary of fragments for a manuscript.
    /// </summary>
    [HttpGet("fragments/{id}")]
    public async Task<ActionResult<List<FragmentSummary>>> GetFragments(long id)
    {
        return await service.FragmentSummaries(id);
    }
}

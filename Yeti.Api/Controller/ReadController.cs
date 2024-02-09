using Microsoft.AspNetCore.Mvc;

using Yeti.Core.Model;
using Yeti.Core.Service;

namespace Yeti.Api.Controller;

[ApiController]
[Route("[controller]")]
public class ReadController(
    ManuscriptService manuscript,
    FragmentService fragment,
    RecentService recentService)
    : ControllerBase
{
    /// <summary>
    /// Gets the newest manuscripts.
    /// </summary>
    [HttpGet("/new")]
    public async Task<ActionResult<List<ManuscriptSummary>>> GetNew()
    {
        return await recentService.ByNew(0);
    }

    /// <summary>
    /// Gets the most recently updated manuscripts.
    /// </summary>
    [HttpGet("/updated")]
    public async Task<ActionResult<List<ManuscriptSummary>>> GetUpdated()
    {
        return await recentService.ByUpdated(0);
    }

    /// <summary>
    /// Retrieves a fragment for rendering.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<FragmentDetail?>> GetFragmentDetail(long id)
    {
        return await fragment.GetFragmentDetail(id);
    }

    /// <summary>
    /// Retrieves a manuscript summary.
    /// </summary>
    [HttpGet("summary/{id}")]
    public async Task<ActionResult<ManuscriptSummary?>> Get(long id)
    {
        return await manuscript.ManuscriptSummary(id);
    }

    /// <summary>
    /// Retrieves a summary of fragments for a manuscript.
    /// </summary>
    [HttpGet("listing/{id}")]
    public async Task<ActionResult<List<FragmentSummary>>> GetFragments(long id)
    {
        return await manuscript.FragmentSummaries(id);
    }
}

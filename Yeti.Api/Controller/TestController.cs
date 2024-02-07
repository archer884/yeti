using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Yeti.Db;
using Yeti.Db.Model;

namespace Yeti.Api.Controller;

[Route("[controller]")]
[ApiController]
public class TestController(WriterContext context)
{
    [HttpGet]
    public async Task<ActionResult<List<Tag>>> GetTags()
    {
        return await context.Tags.ToListAsync();
    }
}
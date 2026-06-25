using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Yeti.Core.Model;
using Yeti.Core.Service;

namespace Yeti.Api.Controller;

[ApiController]
[Authorize]
[Route("[controller]")]
public class WriterController(WriterService service) : YetiController
{
    [HttpGet]
    public async Task<ActionResult<WriterInfo?>> Get() => Ok(service.GetInfo(UserId));

    [HttpPost("add")]
    public async Task<ActionResult<WriterInfo?>> Post()
    {
        throw new NotImplementedException();
    }

    [HttpPost("edit")]
    public async Task<ActionResult<WriterInfo?>> Post([FromBody] UpdateWriter writer)
        => await service.Update(UserId, writer);
}

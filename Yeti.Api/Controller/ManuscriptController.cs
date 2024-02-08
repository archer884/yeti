using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Yeti.Core.Model;
using Yeti.Core.Service;

namespace Yeti.Api.Controller;

[Authorize]
[ApiController]
[Route("[controller]")]
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
}

using Microsoft.AspNetCore.Mvc;

using Yeti.Core.Model;
using Yeti.Core.Service;

namespace Yeti.Api.Controller;

[Route("[controller]")]
[ApiController]
public class TagController(TagService service) : ControllerBase
{
    [HttpPost("add")]
    public async Task AddTag([FromBody] ModifyTag modify)
    {
        await service.AddTag(modify);
    }

    [HttpPost("remove")]
    public async Task RemoveTag([FromBody] ModifyTag modify)
    {
        await service.RemoveTag(modify);
    }
}

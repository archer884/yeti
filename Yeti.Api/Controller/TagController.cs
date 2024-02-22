using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Yeti.Core.Model;
using Yeti.Core.Service;

namespace Yeti.Api.Controller;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TagController(TagService service) : YetiController
{
    [HttpPost("add")]
    public async Task AddTag([FromBody] ModifyTag modify) => await service.AddTag(UserId, modify);

    [HttpPost("remove")]
    public async Task RemoveTag([FromBody] ModifyTag modify) => await service.RemoveTag(UserId, modify);
}

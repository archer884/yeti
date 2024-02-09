using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Yeti.Core.Model;
using Yeti.Core.Service;

namespace Yeti.Api.Controller;

[Authorize]
[ApiController]
[Route("[controller]")]
public class FragmentController(FragmentService service) : YetiController
{
    [HttpPost]
    public async Task<ActionResult<FragmentSummary?>> Post([FromBody] CreateFragment create)
    {
        return await service.CreateFragment(UserId, create);
    }

    [HttpPut]
    public async Task<ActionResult<FragmentSummary?>> Update([FromBody] UpdateFragment update)
    {
        return await service.UpdateFragment(UserId, update);
    }

    [HttpDelete]
    public async Task<ActionResult<FragmentSummary?>> Delete([FromBody] DeleteFragment delete)
    {
        return await service.DeleteFragment(UserId, delete);
    }
}

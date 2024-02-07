using Microsoft.AspNetCore.Mvc;

using Yeti.Core.Model;
using Yeti.Core.Service;

namespace Yeti.Api.Controller;

[Route("[controller]")]
[ApiController]
public class FragmentController(FragmentService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<FragmentSummary?>> Post([FromBody] CreateFragment create)
    {
        return await service.CreateFragment(create);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FragmentDetail?>> Get(long id)
    {
        return await service.GetFragmentDetail(id);
    }

    [HttpPut]
    public async Task<ActionResult<FragmentSummary?>> Update([FromBody] UpdateFragment update)
    {
        return await service.UpdateFragment(update);
    }

    [HttpDelete]
    public async Task<ActionResult<FragmentSummary?>> Delete([FromBody] DeleteFragment delete)
    {
        return await service.DeleteFragment(delete);
    }
}

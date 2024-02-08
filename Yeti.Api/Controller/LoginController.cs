using Microsoft.AspNetCore.Mvc;

using Yeti.Api.Config;
using Yeti.Core.Model;

namespace Yeti.Api.Controller;

[ApiController]
[Route("[controller]")]
public class LoginController(TokenService service) : ControllerBase
{
    [HttpPost]
    public ActionResult<string> Login([FromBody] Login login)
    {
        var (username, password) = login;
        if (username == "test" && password == "test")
        {
            return service.GenerateToken(1);
        }

        // Always be courteous and professional.
        return Unauthorized("Up yours");
    }
}

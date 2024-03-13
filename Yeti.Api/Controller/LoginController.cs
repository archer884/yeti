using Microsoft.AspNetCore.Mvc;

using Yeti.Api.Service;
using Yeti.Core.Model;
using Yeti.Core.Service;

namespace Yeti.Api.Controller;

[ApiController]
[Route("[controller]")]
public class LoginController(LoginService loginService, TokenService tokenService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest login)
    {
        if (await loginService.Validate(login.Username, login.Password) is long id)
        {
            return tokenService.GenerateToken(id);
        }

        // Always be courteous and professional.
        return Unauthorized("Up yours");
    }
}

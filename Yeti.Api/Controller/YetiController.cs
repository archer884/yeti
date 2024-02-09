using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Yeti.Api.Controller;

public class YetiController() : ControllerBase
{
    const string UserIdClaimType = "id";

    protected long UserId => TryGetUserId(User);

    static long TryGetUserId(ClaimsPrincipal user)
    {
        var claim = user.Claims.First(x => x.Type == UserIdClaimType);
        return long.TryParse(claim?.Value, out var id)
            ? id : throw new UnauthorizedAccessException();
    }
}

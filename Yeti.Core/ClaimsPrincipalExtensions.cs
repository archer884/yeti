using System.Security.Claims;

namespace Yeti.Core;

public static class ClaimsPrincipalExtensions
{
    public static long? GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return long.TryParse(value, out var id) ? id : null;
    }
}

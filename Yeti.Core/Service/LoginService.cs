using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Yeti.Core.Provider;
using Yeti.Db;

namespace Yeti.Core.Service;

public class LoginService(ILogger<LoginService> logger, WriterContext context, HashProvider provider)
{
    public async Task<long?> Validate(string username, string password)
    {
        var login = await context.Logins
            .Include(x => x.Writer)
            .Select(x => new { x.WriterId, x.Writer.Username, x.Serialized })
            .FirstOrDefaultAsync(x => x.Username == username);

        if (login is null)
        {
            logger.LogDebug("bad login for {username}", username);
            return null;
        }

        if (provider.VerifyHash(login.Serialized, password))
        {
            logger.LogDebug("LOGIN SUCCEEDED for {id}/{username}", login.WriterId, login.Username);
            return login.WriterId;
        }
        else
        {
            logger.LogDebug("LOGIN FAILED for {id}/{username}", login.WriterId, login.Username);
            return null;
        }
    }
}

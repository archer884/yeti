using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;

using Yeti.Api.Config;
using Yeti.Core;

namespace Yeti.Api.Service;

public class TokenService(IOptions<TokenOptions> options)
{
    static TimeSpan DefaultRefreshLifetime => TimeSpan.FromDays(30);
    static TimeSpan DefaultAccessLifetime => TimeSpan.FromMinutes(15);

    // FIXME: we need some way to differentiate this token from the others.
    // https://code-maze.com/using-refresh-tokens-in-asp-net-core-authentication/
    // https://oauth2simplified.com/
    //
    // Looks like we don't want an actual token for the refresh token; we just want
    // to generate some random garbage and store it in the db for later. The db can
    // track expiration time and, if we want to invalidate a token, we just remove
    // it from the db.
    public string GenerateRefreshToken(long userId, TimeSpan? lifetime = null)
    {
        var token = TokenWithLifetime(userId, lifetime ?? DefaultRefreshLifetime);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateAccessToken(long userId, TimeSpan? lifetime = null)
    {
        var token = TokenWithLifetime(userId, lifetime ?? DefaultAccessLifetime);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    JwtSecurityToken TokenWithLifetime(long userId, TimeSpan lifetime) => new(
        issuer: options.Value.Issuer,
        claims: [new("id", userId.ToString())],
        signingCredentials: options.Value.Credentials,
        expires: Time.Now.DateTime.Add(lifetime)
    );
}

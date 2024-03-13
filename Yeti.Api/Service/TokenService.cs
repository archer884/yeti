using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;

using Yeti.Api.Config;

namespace Yeti.Api.Service;

public class TokenService(IOptions<TokenOptions> options)
{
    public string GenerateToken(long userId)
    {
        var token = new JwtSecurityToken(
            issuer: options.Value.Issuer,
            claims: [new("id", userId.ToString())],
            signingCredentials: options.Value.Credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

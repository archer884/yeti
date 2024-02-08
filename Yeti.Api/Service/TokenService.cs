using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;

namespace Yeti.Api.Config;

public class TokenService(IOptions<TokenOptions> options)
{
    public string GenerateToken(long userId)
    {
        var token = new JwtSecurityToken(
            issuer: options.Value.Issuer,
            claims: [new("userid", userId.ToString())],
            signingCredentials: options.Value.Credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

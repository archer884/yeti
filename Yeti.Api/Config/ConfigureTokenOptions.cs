using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Yeti.Api.Config;

public class ConfigureTokenOptions(IConfiguration configuration) : IConfigureOptions<TokenOptions>
{
    public void Configure(TokenOptions options)
    {
        var keyData = configuration["Jwt:Key"] ?? throw new Exception("Jwt:Key");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyData));

        options.Credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        options.Issuer = configuration["Jwt:Issuer"] ?? throw new Exception("Jwt:Issuer");
    }
}

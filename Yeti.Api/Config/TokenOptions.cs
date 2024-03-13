using System.Text;
using Microsoft.IdentityModel.Tokens;

using Yeti.Core.Config;

namespace Yeti.Api.Config;

public class TokenOptions : IConfigurable
{
    public SigningCredentials Credentials { get; set; } = null!;
    public string Issuer { get; set; } = null!;

    public void Configure(IConfiguration configuration)
    {
        var keyData = configuration["Jwt:Key"] ?? throw new Exception("Jwt:Key");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyData));

        Credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        Issuer = configuration["Jwt:Issuer"] ?? throw new Exception("Jwt:Issuer");
    }
}

using Microsoft.IdentityModel.Tokens;

namespace Yeti.Api.Config;

public class TokenOptions
{
    public SigningCredentials Credentials { get; set; } = null!;
    public string Issuer { get; set; } = null!;
}

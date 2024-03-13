using Microsoft.Extensions.Configuration;

namespace Yeti.Core.Config;

public static class ConfigSections
{
    public static IConfigurationSection Search(this IConfiguration configuration)
        => configuration.GetRequiredSection("Search");

    public static IConfigurationSection Hashing(this IConfiguration configuration)
        => configuration.GetSection("PasswordHashing");
}

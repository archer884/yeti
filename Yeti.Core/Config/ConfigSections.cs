using Microsoft.Extensions.Configuration;

namespace Yeti.Core.Config;

public static class ConfigSections
{
    public static IConfigurationSection Search(this IConfiguration configuration)
        => configuration.GetRequiredSection("Search");
}

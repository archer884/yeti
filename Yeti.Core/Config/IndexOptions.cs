using Microsoft.Extensions.Configuration;

namespace Yeti.Core.Config;

public class IndexOptions : IConfigurable
{
    public string Url { get; set; } = null!;

    public void Configure(IConfiguration configuration)
    {
        configuration.Search().Bind(this);

        if (string.IsNullOrWhiteSpace(Url))
        {
            throw new ConfigurationException("Search:Url");
        }
    }
}

using Microsoft.Extensions.Configuration;

namespace Yeti.Core.Config;

public class IndexOptions : IConfigurable
{
    public string? Url { get; set; }

    public void Configure(IConfiguration configuration)
    {
        configuration.Search().Bind(this);
    }
}

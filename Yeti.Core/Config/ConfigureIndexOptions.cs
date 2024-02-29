using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Yeti.Core.Config;

public class ConfigureIndexOptions(IConfiguration configuration) : IConfigureOptions<IndexOptions>
{
    public void Configure(IndexOptions options)
    {
        configuration.Search().Bind(options);
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Yeti.Core.Config;

public class Configure<T>(IConfiguration configuration) : IConfigureOptions<T>
    where T : class, IConfigurable
{
    void IConfigureOptions<T>.Configure(T options)
    {
        options.Configure(configuration);
    }
}

using Microsoft.Extensions.Configuration;

namespace Yeti.Core.Config;

public class HashProviderOptions : IConfigurable
{
    /// <summary>
    /// Defaults to 64 megabytes...
    /// </summary>
    public int Memory { get; set; } = 64 << 20;
    public int Iterations { get; set; } = 3;

    // libsodium ONLY supports parallelism of 1
    // public int Parallelism { get; set; } = 1;

    public void Configure(IConfiguration configuration)
    {
        configuration.Hashing().Bind(this);
    }
}

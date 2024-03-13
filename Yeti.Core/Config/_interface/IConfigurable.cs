using Microsoft.Extensions.Configuration;

namespace Yeti.Core.Config;

public interface IConfigurable
{
    public void Configure(IConfiguration configuration);
}

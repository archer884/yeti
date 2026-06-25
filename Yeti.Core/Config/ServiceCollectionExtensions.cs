using Microsoft.Extensions.DependencyInjection;

namespace Yeti.Core.Config;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigurable<T>(this IServiceCollection services)
        where T : class, IConfigurable
    {
        return services.ConfigureOptions<Configure<T>>();
    }
}

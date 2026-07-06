using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyWay.EF.DependencyInjection;

public static class EfServiceCollectionExtensions
{
    public static IServiceCollection AddMyWayEf(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        _ = configuration;

        return services;
    }
}

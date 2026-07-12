using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyWay.Application.Abstractions.Services;
using MyWay.Infrastructure.Time;

namespace MyWay.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddMyWayInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        _ = configuration;

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }
}

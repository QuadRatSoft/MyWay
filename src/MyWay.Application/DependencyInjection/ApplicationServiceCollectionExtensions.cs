using Microsoft.Extensions.DependencyInjection;

namespace MyWay.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddMyWayApplication(this IServiceCollection services)
    {
        return services;
    }
}

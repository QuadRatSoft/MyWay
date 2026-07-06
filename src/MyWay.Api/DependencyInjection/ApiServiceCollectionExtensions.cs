using Microsoft.Extensions.DependencyInjection;
using MyWay.Api.Controllers;

namespace MyWay.Api.DependencyInjection;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddMyWayApi(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddApplicationPart(typeof(SystemController).Assembly);

        return services;
    }
}

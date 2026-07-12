using Microsoft.Extensions.DependencyInjection;
using MyWay.Api.CurrentUser;
using MyWay.Api.Controllers;
using MyWay.Application.Abstractions.Services;

namespace MyWay.Api.DependencyInjection;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddMyWayApi(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddApplicationPart(typeof(SystemController).Assembly);

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, HeaderCurrentUserContext>();

        return services;
    }
}

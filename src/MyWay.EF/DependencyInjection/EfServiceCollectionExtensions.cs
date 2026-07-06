using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyWay.Application.Abstractions;

namespace MyWay.EF.DependencyInjection;

public static class EfServiceCollectionExtensions
{
    public static IServiceCollection AddMyWayEf(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("MyWay")
            ?? configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'MyWay' or 'DefaultConnection' is required.");
        }

        services.AddDbContext<MyWayDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        return services;
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyWay.Application.Abstractions;
using MyWay.Application.Abstractions.Repositories;
using MyWay.EF.Repositories;

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
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<ICompanyRepository, EfCompanyRepository>();
        services.AddScoped<ICompanyMemberRepository, EfCompanyMemberRepository>();
        services.AddScoped<ICustomerProfileRepository, EfCustomerProfileRepository>();
        services.AddScoped<ICarrierProfileRepository, EfCarrierProfileRepository>();
        services.AddScoped<IDriverProfileRepository, EfDriverProfileRepository>();
        services.AddScoped<IVehicleRepository, EfVehicleRepository>();
        services.AddScoped<IWarehouseRepository, EfWarehouseRepository>();
        services.AddScoped<IShipmentRequestRepository, EfShipmentRequestRepository>();
        services.AddScoped<IShipmentOfferRepository, EfShipmentOfferRepository>();
        services.AddScoped<IShipmentOrderRepository, EfShipmentOrderRepository>();
        services.AddScoped<ICarrierListingRepository, EfCarrierListingRepository>();
        services.AddScoped<IResourceReservationRepository, EfResourceReservationRepository>();
        services.AddScoped<IWaybillRepository, EfWaybillRepository>();
        services.AddScoped<IReviewRepository, EfReviewRepository>();

        return services;
    }
}

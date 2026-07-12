using Microsoft.Extensions.DependencyInjection;
using MyWay.Application.Abstractions.Services;
using MyWay.Application.Services;
using MyWay.Application.UseCases.Boards;
using MyWay.Application.UseCases.ShipmentOffers;
using MyWay.Application.UseCases.ShipmentOrders;
using MyWay.Application.UseCases.ShipmentRequests;

namespace MyWay.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddMyWayApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserAccessService, UserAccessService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();

        services.AddScoped<CreatePublicShipmentRequestUseCase>();
        services.AddScoped<CreateDirectShipmentRequestUseCase>();
        services.AddScoped<PublishShipmentRequestUseCase>();
        services.AddScoped<CancelShipmentRequestUseCase>();

        services.AddScoped<CreateShipmentOfferUseCase>();
        services.AddScoped<AcceptShipmentOfferUseCase>();
        services.AddScoped<RejectShipmentOfferUseCase>();
        services.AddScoped<WithdrawShipmentOfferUseCase>();

        services.AddScoped<SearchShipmentRequestBoardUseCase>();
        services.AddScoped<SearchCarrierListingBoardUseCase>();
        services.AddScoped<GetShipmentRequestDetailsUseCase>();
        services.AddScoped<GetCarrierListingDetailsUseCase>();

        services.AddScoped<AssignShipmentOrderResourcesUseCase>();
        services.AddScoped<StartShipmentOrderUseCase>();
        services.AddScoped<MarkShipmentOrderDeliveredUseCase>();
        services.AddScoped<CompleteShipmentOrderUseCase>();
        services.AddScoped<CancelShipmentOrderUseCase>();

        return services;
    }
}

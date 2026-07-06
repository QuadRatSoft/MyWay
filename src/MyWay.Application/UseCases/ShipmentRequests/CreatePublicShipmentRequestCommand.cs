using MyWay.Core.Common.ValueObjects;

namespace MyWay.Application.UseCases.ShipmentRequests;

public sealed record CreatePublicShipmentRequestCommand(
    Guid? CustomerUserId,
    Guid? CustomerCompanyId,
    Address PickupAddress,
    Address DeliveryAddress,
    CargoDetails CargoDetails,
    Money CustomerPrice,
    DateTimeOffset PlannedPickupAt,
    DateTimeOffset PlannedDeliveryAt);

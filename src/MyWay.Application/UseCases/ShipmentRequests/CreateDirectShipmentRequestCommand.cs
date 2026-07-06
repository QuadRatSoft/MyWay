using MyWay.Core.Common.ValueObjects;

namespace MyWay.Application.UseCases.ShipmentRequests;

public sealed record CreateDirectShipmentRequestCommand(
    Guid? CustomerUserId,
    Guid? CustomerCompanyId,
    Guid TargetCarrierListingId,
    Address PickupAddress,
    Address DeliveryAddress,
    CargoDetails CargoDetails,
    Money CustomerPrice,
    DateTimeOffset PlannedPickupAt,
    DateTimeOffset PlannedDeliveryAt);

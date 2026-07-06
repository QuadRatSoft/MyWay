using MyWay.Core.Common.ValueObjects;

namespace MyWay.Application.UseCases.ShipmentOffers;

public sealed record CreateShipmentOfferCommand(
    Guid ShipmentRequestId,
    Guid? CarrierUserId,
    Guid? CarrierCompanyId,
    Money OfferedPrice,
    string? Comment);

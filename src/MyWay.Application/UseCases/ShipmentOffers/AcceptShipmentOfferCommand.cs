namespace MyWay.Application.UseCases.ShipmentOffers;

public sealed record AcceptShipmentOfferCommand(Guid ShipmentRequestId, Guid ShipmentOfferId);

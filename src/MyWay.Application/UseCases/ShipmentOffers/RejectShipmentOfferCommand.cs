namespace MyWay.Application.UseCases.ShipmentOffers;

public sealed record RejectShipmentOfferCommand(Guid ShipmentRequestId, Guid ShipmentOfferId);

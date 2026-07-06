namespace MyWay.Application.UseCases.Boards;

public sealed record ShipmentRequestBoardItem(
    Guid Id,
    Guid CreatedByUserId,
    Guid? CustomerUserId,
    Guid? CustomerCompanyId,
    string PickupCity,
    string DeliveryCity,
    DateTimeOffset PlannedPickupAt,
    DateTimeOffset PlannedDeliveryAt,
    decimal CustomerPriceAmount,
    string CustomerPriceCurrency,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PublishedAt);

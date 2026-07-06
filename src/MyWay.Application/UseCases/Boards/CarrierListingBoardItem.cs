namespace MyWay.Application.UseCases.Boards;

public sealed record CarrierListingBoardItem(
    Guid Id,
    Guid? CarrierUserId,
    Guid? CarrierCompanyId,
    string Title,
    string Status,
    bool IsVisibleOnCarrierBoard,
    decimal? PriceAmount,
    string? PriceCurrency,
    DateTimeOffset CreatedAt,
    DateTimeOffset? AvailableAt);

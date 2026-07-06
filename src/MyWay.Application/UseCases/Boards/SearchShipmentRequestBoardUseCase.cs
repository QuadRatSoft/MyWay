using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Common;
using MyWay.Application.Common.Pagination;
using MyWay.Core.Shipments;

namespace MyWay.Application.UseCases.Boards;

public sealed class SearchShipmentRequestBoardUseCase
{
    private readonly IShipmentRequestRepository shipmentRequestRepository;

    public SearchShipmentRequestBoardUseCase(IShipmentRequestRepository shipmentRequestRepository)
    {
        this.shipmentRequestRepository = shipmentRequestRepository;
    }

    public async Task<Result<PagedResult<ShipmentRequestBoardItem>>> ExecuteAsync(
        SearchShipmentRequestBoardQuery query,
        CancellationToken cancellationToken = default)
    {
        var pagedRequests = await shipmentRequestRepository.SearchPublishedForBoardAsync(
            query.Filter,
            query.Pagination,
            cancellationToken);

        var items = pagedRequests.Items
            .Select(MapToBoardItem)
            .ToArray();

        var result = PagedResult<ShipmentRequestBoardItem>.Create(
            items,
            pagedRequests.PageNumber,
            pagedRequests.PageSize,
            pagedRequests.TotalCount);

        return Result<PagedResult<ShipmentRequestBoardItem>>.Success(result);
    }

    private static ShipmentRequestBoardItem MapToBoardItem(ShipmentRequest request)
    {
        return new ShipmentRequestBoardItem(
            request.Id,
            request.CreatedByUserId,
            request.CustomerUserId,
            request.CustomerCompanyId,
            request.PickupAddress.City,
            request.DeliveryAddress.City,
            request.PlannedPickupAt,
            request.PlannedDeliveryAt,
            request.CustomerPrice.Amount,
            request.CustomerPrice.Currency,
            request.Status.ToString(),
            request.CreatedAt,
            request.PublishedAt);
    }
}

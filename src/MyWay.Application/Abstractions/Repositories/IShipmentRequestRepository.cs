using MyWay.Application.Common.Pagination;
using MyWay.Application.UseCases.Boards;
using MyWay.Core.Shipments;

namespace MyWay.Application.Abstractions.Repositories;

public interface IShipmentRequestRepository
{
    Task<ShipmentRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<ShipmentRequest>> SearchPublishedForBoardAsync(
        ShipmentRequestBoardFilter filter,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task AddAsync(ShipmentRequest entity, CancellationToken cancellationToken = default);
}

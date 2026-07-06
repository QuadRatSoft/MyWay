using MyWay.Application.Common.Pagination;
using MyWay.Application.UseCases.Boards;
using MyWay.Core.CarrierListings;

namespace MyWay.Application.Abstractions.Repositories;

public interface ICarrierListingRepository
{
    Task<CarrierListing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<CarrierListing>> SearchVisibleForBoardAsync(
        CarrierListingBoardFilter filter,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task AddAsync(CarrierListing entity, CancellationToken cancellationToken = default);
}

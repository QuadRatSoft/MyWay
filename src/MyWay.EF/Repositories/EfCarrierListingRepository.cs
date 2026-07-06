using Microsoft.EntityFrameworkCore;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Common.Pagination;
using MyWay.Application.UseCases.Boards;
using MyWay.Core.CarrierListings;

namespace MyWay.EF.Repositories;

public sealed class EfCarrierListingRepository : ICarrierListingRepository
{
    private readonly MyWayDbContext dbContext;

    public EfCarrierListingRepository(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<CarrierListing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.CarrierListings.FirstOrDefaultAsync(listing => listing.Id == id, cancellationToken);
    }

    public async Task<PagedResult<CarrierListing>> SearchVisibleForBoardAsync(
        CarrierListingBoardFilter filter,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.CarrierListings
            .AsNoTracking()
            .Where(listing => listing.Status == CarrierListingStatus.Available);

        if (filter.MinPriceAmount.HasValue)
        {
            query = query.Where(listing =>
                listing.BasePrice != null && listing.BasePrice.Amount >= filter.MinPriceAmount.Value);
        }

        if (filter.MaxPriceAmount.HasValue)
        {
            query = query.Where(listing =>
                listing.BasePrice != null && listing.BasePrice.Amount <= filter.MaxPriceAmount.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Currency))
        {
            var currency = filter.Currency.Trim();
            query = query.Where(listing =>
                listing.BasePrice != null
                && Microsoft.EntityFrameworkCore.EF.Functions.ILike(listing.BasePrice.Currency, currency));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(listing => listing.CreatedAt)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArrayAsync(cancellationToken);

        return PagedResult<CarrierListing>.Create(
            items,
            pagination.PageNumber,
            pagination.PageSize,
            totalCount);
    }

    public async Task AddAsync(CarrierListing entity, CancellationToken cancellationToken = default)
    {
        await dbContext.CarrierListings.AddAsync(entity, cancellationToken);
    }
}

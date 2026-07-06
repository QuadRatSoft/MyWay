using Microsoft.EntityFrameworkCore;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Common.Pagination;
using MyWay.Application.UseCases.Boards;
using MyWay.Core.Shipments;

namespace MyWay.EF.Repositories;

public sealed class EfShipmentRequestRepository : IShipmentRequestRepository
{
    private readonly MyWayDbContext dbContext;

    public EfShipmentRequestRepository(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<ShipmentRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.ShipmentRequests.FirstOrDefaultAsync(request => request.Id == id, cancellationToken);
    }

    public async Task<PagedResult<ShipmentRequest>> SearchPublishedForBoardAsync(
        ShipmentRequestBoardFilter filter,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.ShipmentRequests
            .AsNoTracking()
            .Where(request => request.Status == ShipmentRequestStatus.Published);

        if (!string.IsNullOrWhiteSpace(filter.PickupCity))
        {
            var pickupCity = filter.PickupCity.Trim();
            query = query.Where(request => Microsoft.EntityFrameworkCore.EF.Functions.ILike(
                request.PickupAddress.City,
                pickupCity));
        }

        if (!string.IsNullOrWhiteSpace(filter.DeliveryCity))
        {
            var deliveryCity = filter.DeliveryCity.Trim();
            query = query.Where(request => Microsoft.EntityFrameworkCore.EF.Functions.ILike(
                request.DeliveryAddress.City,
                deliveryCity));
        }

        if (filter.PickupFrom.HasValue)
        {
            query = query.Where(request => request.PlannedPickupAt >= filter.PickupFrom.Value);
        }

        if (filter.PickupTo.HasValue)
        {
            query = query.Where(request => request.PlannedPickupAt <= filter.PickupTo.Value);
        }

        if (filter.MinPriceAmount.HasValue)
        {
            query = query.Where(request => request.CustomerPrice.Amount >= filter.MinPriceAmount.Value);
        }

        if (filter.MaxPriceAmount.HasValue)
        {
            query = query.Where(request => request.CustomerPrice.Amount <= filter.MaxPriceAmount.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Currency))
        {
            var currency = filter.Currency.Trim();
            query = query.Where(request => Microsoft.EntityFrameworkCore.EF.Functions.ILike(
                request.CustomerPrice.Currency,
                currency));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(request => request.CreatedAt)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArrayAsync(cancellationToken);

        return PagedResult<ShipmentRequest>.Create(
            items,
            pagination.PageNumber,
            pagination.PageSize,
            totalCount);
    }

    public async Task AddAsync(ShipmentRequest entity, CancellationToken cancellationToken = default)
    {
        await dbContext.ShipmentRequests.AddAsync(entity, cancellationToken);
    }
}

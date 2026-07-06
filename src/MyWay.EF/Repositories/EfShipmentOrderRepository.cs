using Microsoft.EntityFrameworkCore;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Core.Shipments;

namespace MyWay.EF.Repositories;

public sealed class EfShipmentOrderRepository : IShipmentOrderRepository
{
    private readonly MyWayDbContext dbContext;

    public EfShipmentOrderRepository(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<ShipmentOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.ShipmentOrders.FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ShipmentOrder>> GetByCustomerUserIdAsync(
        Guid customerUserId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ShipmentOrders
            .Where(order => order.CustomerUserId == customerUserId)
            .OrderBy(order => order.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ShipmentOrder>> GetByCustomerCompanyIdAsync(
        Guid customerCompanyId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ShipmentOrders
            .Where(order => order.CustomerCompanyId == customerCompanyId)
            .OrderBy(order => order.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ShipmentOrder>> GetByCarrierUserIdAsync(
        Guid carrierUserId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ShipmentOrders
            .Where(order => order.CarrierUserId == carrierUserId)
            .OrderBy(order => order.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ShipmentOrder>> GetByCarrierCompanyIdAsync(
        Guid carrierCompanyId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ShipmentOrders
            .Where(order => order.CarrierCompanyId == carrierCompanyId)
            .OrderBy(order => order.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(ShipmentOrder entity, CancellationToken cancellationToken = default)
    {
        await dbContext.ShipmentOrders.AddAsync(entity, cancellationToken);
    }
}

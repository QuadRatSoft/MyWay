using Microsoft.EntityFrameworkCore;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Core.Warehouses;

namespace MyWay.EF.Repositories;

public sealed class EfWarehouseRepository : IWarehouseRepository
{
    private readonly MyWayDbContext dbContext;

    public EfWarehouseRepository(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Warehouses.FirstOrDefaultAsync(warehouse => warehouse.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Warehouse>> GetByOwnerUserIdAsync(
        Guid ownerUserId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Warehouses
            .Where(warehouse => warehouse.OwnerUserId == ownerUserId)
            .OrderBy(warehouse => warehouse.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Warehouse>> GetByOwnerCompanyIdAsync(
        Guid ownerCompanyId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Warehouses
            .Where(warehouse => warehouse.OwnerCompanyId == ownerCompanyId)
            .OrderBy(warehouse => warehouse.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(Warehouse entity, CancellationToken cancellationToken = default)
    {
        await dbContext.Warehouses.AddAsync(entity, cancellationToken);
    }
}

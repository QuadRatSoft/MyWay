using Microsoft.EntityFrameworkCore;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Core.Vehicles;

namespace MyWay.EF.Repositories;

public sealed class EfVehicleRepository : IVehicleRepository
{
    private readonly MyWayDbContext dbContext;

    public EfVehicleRepository(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Vehicles.FirstOrDefaultAsync(vehicle => vehicle.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Vehicle>> GetByOwnerUserIdAsync(
        Guid ownerUserId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Vehicles
            .Where(vehicle => vehicle.OwnerUserId == ownerUserId)
            .OrderBy(vehicle => vehicle.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Vehicle>> GetByOwnerCompanyIdAsync(
        Guid ownerCompanyId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Vehicles
            .Where(vehicle => vehicle.OwnerCompanyId == ownerCompanyId)
            .OrderBy(vehicle => vehicle.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(Vehicle entity, CancellationToken cancellationToken = default)
    {
        await dbContext.Vehicles.AddAsync(entity, cancellationToken);
    }
}

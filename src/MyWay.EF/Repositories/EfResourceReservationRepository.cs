using Microsoft.EntityFrameworkCore;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Core.Resources;

namespace MyWay.EF.Repositories;

public sealed class EfResourceReservationRepository : IResourceReservationRepository
{
    private readonly MyWayDbContext dbContext;

    public EfResourceReservationRepository(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<ResourceReservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.ResourceReservations.FirstOrDefaultAsync(reservation => reservation.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ResourceReservation>> GetActiveByDriverUserIdAsync(
        Guid driverUserId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ResourceReservations
            .Where(reservation =>
                reservation.Status == ResourceReservationStatus.Active && reservation.DriverUserId == driverUserId)
            .OrderBy(reservation => reservation.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ResourceReservation>> GetActiveByVehicleIdAsync(
        Guid vehicleId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ResourceReservations
            .Where(reservation =>
                reservation.Status == ResourceReservationStatus.Active && reservation.VehicleId == vehicleId)
            .OrderBy(reservation => reservation.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ResourceReservation>> GetActiveByShipmentOrderIdAsync(
        Guid shipmentOrderId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ResourceReservations
            .Where(reservation =>
                reservation.Status == ResourceReservationStatus.Active && reservation.ShipmentOrderId == shipmentOrderId)
            .OrderBy(reservation => reservation.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(ResourceReservation entity, CancellationToken cancellationToken = default)
    {
        await dbContext.ResourceReservations.AddAsync(entity, cancellationToken);
    }
}

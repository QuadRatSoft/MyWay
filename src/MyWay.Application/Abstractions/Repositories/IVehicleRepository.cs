using MyWay.Core.Vehicles;

namespace MyWay.Application.Abstractions.Repositories;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Vehicle>> GetByOwnerUserIdAsync(
        Guid ownerUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Vehicle>> GetByOwnerCompanyIdAsync(
        Guid ownerCompanyId,
        CancellationToken cancellationToken = default);

    Task AddAsync(Vehicle entity, CancellationToken cancellationToken = default);
}

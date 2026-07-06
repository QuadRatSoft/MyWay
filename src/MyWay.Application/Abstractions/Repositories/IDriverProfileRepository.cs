using MyWay.Core.Profiles;

namespace MyWay.Application.Abstractions.Repositories;

public interface IDriverProfileRepository
{
    Task<DriverProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DriverProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(DriverProfile entity, CancellationToken cancellationToken = default);
}

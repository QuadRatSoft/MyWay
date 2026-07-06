using MyWay.Core.Profiles;

namespace MyWay.Application.Abstractions.Repositories;

public interface ICarrierProfileRepository
{
    Task<CarrierProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CarrierProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<CarrierProfile?> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);

    Task AddAsync(CarrierProfile entity, CancellationToken cancellationToken = default);
}

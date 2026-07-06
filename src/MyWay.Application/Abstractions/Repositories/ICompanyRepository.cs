using MyWay.Core.Companies;

namespace MyWay.Application.Abstractions.Repositories;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Company entity, CancellationToken cancellationToken = default);
}

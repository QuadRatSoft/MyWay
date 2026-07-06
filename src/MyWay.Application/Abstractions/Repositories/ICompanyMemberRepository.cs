using MyWay.Core.Companies;

namespace MyWay.Application.Abstractions.Repositories;

public interface ICompanyMemberRepository
{
    Task<CompanyMember?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CompanyMember>> GetActiveMembersByCompanyIdAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CompanyMember>> GetActiveMembershipsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<CompanyMember?> GetActiveMembershipAsync(
        Guid companyId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(CompanyMember entity, CancellationToken cancellationToken = default);
}

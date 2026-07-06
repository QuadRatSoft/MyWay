using Microsoft.EntityFrameworkCore;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Core.Companies;

namespace MyWay.EF.Repositories;

public sealed class EfCompanyMemberRepository : ICompanyMemberRepository
{
    private readonly MyWayDbContext dbContext;

    public EfCompanyMemberRepository(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<CompanyMember?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.CompanyMembers.FirstOrDefaultAsync(member => member.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<CompanyMember>> GetActiveMembersByCompanyIdAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.CompanyMembers
            .Where(member => member.CompanyId == companyId && member.IsActive)
            .OrderBy(member => member.JoinedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<CompanyMember>> GetActiveMembershipsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.CompanyMembers
            .Where(member => member.UserId == userId && member.IsActive)
            .OrderBy(member => member.JoinedAt)
            .ToArrayAsync(cancellationToken);
    }

    public Task<CompanyMember?> GetActiveMembershipAsync(
        Guid companyId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.CompanyMembers.FirstOrDefaultAsync(
            member => member.CompanyId == companyId && member.UserId == userId && member.IsActive,
            cancellationToken);
    }

    public async Task AddAsync(CompanyMember entity, CancellationToken cancellationToken = default)
    {
        await dbContext.CompanyMembers.AddAsync(entity, cancellationToken);
    }
}

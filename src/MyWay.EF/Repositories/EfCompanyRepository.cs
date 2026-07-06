using Microsoft.EntityFrameworkCore;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Core.Companies;

namespace MyWay.EF.Repositories;

public sealed class EfCompanyRepository : ICompanyRepository
{
    private readonly MyWayDbContext dbContext;

    public EfCompanyRepository(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Companies.FirstOrDefaultAsync(company => company.Id == id, cancellationToken);
    }

    public async Task AddAsync(Company entity, CancellationToken cancellationToken = default)
    {
        await dbContext.Companies.AddAsync(entity, cancellationToken);
    }
}

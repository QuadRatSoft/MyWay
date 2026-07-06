using Microsoft.EntityFrameworkCore;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Core.Profiles;

namespace MyWay.EF.Repositories;

public sealed class EfCarrierProfileRepository : ICarrierProfileRepository
{
    private readonly MyWayDbContext dbContext;

    public EfCarrierProfileRepository(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<CarrierProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.CarrierProfiles.FirstOrDefaultAsync(profile => profile.Id == id, cancellationToken);
    }

    public Task<CarrierProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return dbContext.CarrierProfiles.FirstOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);
    }

    public Task<CarrierProfile?> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return dbContext.CarrierProfiles.FirstOrDefaultAsync(profile => profile.CompanyId == companyId, cancellationToken);
    }

    public async Task AddAsync(CarrierProfile entity, CancellationToken cancellationToken = default)
    {
        await dbContext.CarrierProfiles.AddAsync(entity, cancellationToken);
    }
}

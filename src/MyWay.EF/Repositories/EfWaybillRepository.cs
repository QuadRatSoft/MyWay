using Microsoft.EntityFrameworkCore;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Core.Waybills;

namespace MyWay.EF.Repositories;

public sealed class EfWaybillRepository : IWaybillRepository
{
    private readonly MyWayDbContext dbContext;

    public EfWaybillRepository(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<Waybill?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Waybills.FirstOrDefaultAsync(waybill => waybill.Id == id, cancellationToken);
    }

    public async Task AddAsync(Waybill entity, CancellationToken cancellationToken = default)
    {
        await dbContext.Waybills.AddAsync(entity, cancellationToken);
    }
}

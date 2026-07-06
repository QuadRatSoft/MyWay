using Microsoft.EntityFrameworkCore;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Core.Shipments;

namespace MyWay.EF.Repositories;

public sealed class EfShipmentOfferRepository : IShipmentOfferRepository
{
    private readonly MyWayDbContext dbContext;

    public EfShipmentOfferRepository(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<ShipmentOffer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.ShipmentOffers.FirstOrDefaultAsync(offer => offer.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ShipmentOffer>> GetByShipmentRequestIdAsync(
        Guid shipmentRequestId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ShipmentOffers
            .Where(offer => offer.ShipmentRequestId == shipmentRequestId)
            .OrderBy(offer => offer.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(ShipmentOffer entity, CancellationToken cancellationToken = default)
    {
        await dbContext.ShipmentOffers.AddAsync(entity, cancellationToken);
    }
}

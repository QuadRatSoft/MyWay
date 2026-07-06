using MyWay.Core.CarrierListings;

namespace MyWay.Application.Abstractions.Repositories;

public interface ICarrierListingRepository
{
    Task<CarrierListing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(CarrierListing entity, CancellationToken cancellationToken = default);
}

using MyWay.Application.Abstractions;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Abstractions.Services;
using MyWay.Core.CarrierListings;
using MyWay.Core.Shipments;

namespace MyWay.UnitTests.TestsSupport.Fakes;

internal sealed class FakeCurrentUserContext : ICurrentUserContext
{
    public bool IsAuthenticated { get; set; } = true;

    public Guid? UserId { get; set; } = Guid.NewGuid();

    public Guid? AuthUserId { get; set; } = Guid.NewGuid();
}

internal sealed class FakeUserAccessService : IUserAccessService
{
    public bool CanActAsCustomerResult { get; set; } = true;

    public bool CanActAsCarrierResult { get; set; } = true;

    public bool CanManageCompanyResult { get; set; } = true;

    public bool IsCompanyMemberResult { get; set; } = true;

    public Task<bool> CanActAsCustomerAsync(
        Guid currentUserId,
        Guid? customerUserId,
        Guid? customerCompanyId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(CanActAsCustomerResult);
    }

    public Task<bool> CanActAsCarrierAsync(
        Guid currentUserId,
        Guid? carrierUserId,
        Guid? carrierCompanyId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(CanActAsCarrierResult);
    }

    public Task<bool> CanManageCompanyAsync(
        Guid currentUserId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(CanManageCompanyResult);
    }

    public Task<bool> IsCompanyMemberAsync(
        Guid currentUserId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(IsCompanyMemberResult);
    }
}

internal sealed class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; set; } = new(2026, 7, 6, 12, 0, 0, TimeSpan.Zero);
}

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveChangesCallCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCallCount++;

        return Task.FromResult(1);
    }
}

internal sealed class InMemoryShipmentRequestRepository : IShipmentRequestRepository
{
    private readonly Dictionary<Guid, ShipmentRequest> requests = [];

    public IReadOnlyCollection<ShipmentRequest> Requests => requests.Values.ToArray();

    public Task<ShipmentRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        requests.TryGetValue(id, out var request);

        return Task.FromResult(request);
    }

    public Task AddAsync(ShipmentRequest entity, CancellationToken cancellationToken = default)
    {
        requests[entity.Id] = entity;

        return Task.CompletedTask;
    }
}

internal sealed class InMemoryShipmentOfferRepository : IShipmentOfferRepository
{
    private readonly Dictionary<Guid, ShipmentOffer> offers = [];

    public IReadOnlyCollection<ShipmentOffer> Offers => offers.Values.ToArray();

    public Task<ShipmentOffer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        offers.TryGetValue(id, out var offer);

        return Task.FromResult(offer);
    }

    public Task<IReadOnlyCollection<ShipmentOffer>> GetByShipmentRequestIdAsync(
        Guid shipmentRequestId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<ShipmentOffer> result = offers.Values
            .Where(offer => offer.ShipmentRequestId == shipmentRequestId)
            .ToArray();

        return Task.FromResult(result);
    }

    public Task AddAsync(ShipmentOffer entity, CancellationToken cancellationToken = default)
    {
        offers[entity.Id] = entity;

        return Task.CompletedTask;
    }
}

internal sealed class InMemoryShipmentOrderRepository : IShipmentOrderRepository
{
    private readonly Dictionary<Guid, ShipmentOrder> orders = [];

    public IReadOnlyCollection<ShipmentOrder> Orders => orders.Values.ToArray();

    public Task<ShipmentOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        orders.TryGetValue(id, out var order);

        return Task.FromResult(order);
    }

    public Task<IReadOnlyCollection<ShipmentOrder>> GetByCustomerUserIdAsync(
        Guid customerUserId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<ShipmentOrder> result = orders.Values
            .Where(order => order.CustomerUserId == customerUserId)
            .ToArray();

        return Task.FromResult(result);
    }

    public Task<IReadOnlyCollection<ShipmentOrder>> GetByCustomerCompanyIdAsync(
        Guid customerCompanyId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<ShipmentOrder> result = orders.Values
            .Where(order => order.CustomerCompanyId == customerCompanyId)
            .ToArray();

        return Task.FromResult(result);
    }

    public Task<IReadOnlyCollection<ShipmentOrder>> GetByCarrierUserIdAsync(
        Guid carrierUserId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<ShipmentOrder> result = orders.Values
            .Where(order => order.CarrierUserId == carrierUserId)
            .ToArray();

        return Task.FromResult(result);
    }

    public Task<IReadOnlyCollection<ShipmentOrder>> GetByCarrierCompanyIdAsync(
        Guid carrierCompanyId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<ShipmentOrder> result = orders.Values
            .Where(order => order.CarrierCompanyId == carrierCompanyId)
            .ToArray();

        return Task.FromResult(result);
    }

    public Task AddAsync(ShipmentOrder entity, CancellationToken cancellationToken = default)
    {
        orders[entity.Id] = entity;

        return Task.CompletedTask;
    }
}

internal sealed class InMemoryCarrierListingRepository : ICarrierListingRepository
{
    private readonly Dictionary<Guid, CarrierListing> carrierListings = [];

    public IReadOnlyCollection<CarrierListing> CarrierListings => carrierListings.Values.ToArray();

    public Task<CarrierListing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        carrierListings.TryGetValue(id, out var carrierListing);

        return Task.FromResult(carrierListing);
    }

    public Task AddAsync(CarrierListing entity, CancellationToken cancellationToken = default)
    {
        carrierListings[entity.Id] = entity;

        return Task.CompletedTask;
    }
}

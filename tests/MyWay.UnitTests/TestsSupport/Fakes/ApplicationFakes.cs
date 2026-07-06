using MyWay.Application.Abstractions;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Abstractions.Services;
using MyWay.Application.Common.Pagination;
using MyWay.Application.UseCases.Boards;
using MyWay.Core.CarrierListings;
using MyWay.Core.Companies;
using MyWay.Core.Resources;
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

    public Task<PagedResult<ShipmentRequest>> SearchPublishedForBoardAsync(
        ShipmentRequestBoardFilter filter,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        var query = requests.Values
            .Where(request => request.Status == ShipmentRequestStatus.Published);

        if (!string.IsNullOrWhiteSpace(filter.PickupCity))
        {
            query = query.Where(request => string.Equals(
                request.PickupAddress.City,
                filter.PickupCity,
                StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filter.DeliveryCity))
        {
            query = query.Where(request => string.Equals(
                request.DeliveryAddress.City,
                filter.DeliveryCity,
                StringComparison.OrdinalIgnoreCase));
        }

        if (filter.PickupFrom.HasValue)
        {
            query = query.Where(request => request.PlannedPickupAt >= filter.PickupFrom.Value);
        }

        if (filter.PickupTo.HasValue)
        {
            query = query.Where(request => request.PlannedPickupAt <= filter.PickupTo.Value);
        }

        if (filter.MinPriceAmount.HasValue)
        {
            query = query.Where(request => request.CustomerPrice.Amount >= filter.MinPriceAmount.Value);
        }

        if (filter.MaxPriceAmount.HasValue)
        {
            query = query.Where(request => request.CustomerPrice.Amount <= filter.MaxPriceAmount.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Currency))
        {
            query = query.Where(request => string.Equals(
                request.CustomerPrice.Currency,
                filter.Currency,
                StringComparison.OrdinalIgnoreCase));
        }

        var matchingItems = query
            .OrderBy(request => request.CreatedAt)
            .ToArray();
        var items = matchingItems
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArray();

        return Task.FromResult(PagedResult<ShipmentRequest>.Create(
            items,
            pagination.PageNumber,
            pagination.PageSize,
            matchingItems.Length));
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

    public Task<PagedResult<CarrierListing>> SearchVisibleForBoardAsync(
        CarrierListingBoardFilter filter,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        var query = carrierListings.Values
            .Where(carrierListing => carrierListing.IsVisibleOnCarrierBoard);

        if (filter.MinPriceAmount.HasValue)
        {
            query = query.Where(carrierListing =>
                carrierListing.BasePrice is not null
                && carrierListing.BasePrice.Amount >= filter.MinPriceAmount.Value);
        }

        if (filter.MaxPriceAmount.HasValue)
        {
            query = query.Where(carrierListing =>
                carrierListing.BasePrice is not null
                && carrierListing.BasePrice.Amount <= filter.MaxPriceAmount.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Currency))
        {
            query = query.Where(carrierListing =>
                carrierListing.BasePrice is not null
                && string.Equals(
                    carrierListing.BasePrice.Currency,
                    filter.Currency,
                    StringComparison.OrdinalIgnoreCase));
        }

        var matchingItems = query
            .OrderBy(carrierListing => carrierListing.CreatedAt)
            .ToArray();
        var items = matchingItems
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArray();

        return Task.FromResult(PagedResult<CarrierListing>.Create(
            items,
            pagination.PageNumber,
            pagination.PageSize,
            matchingItems.Length));
    }

    public Task AddAsync(CarrierListing entity, CancellationToken cancellationToken = default)
    {
        carrierListings[entity.Id] = entity;

        return Task.CompletedTask;
    }
}

internal sealed class InMemoryCompanyMemberRepository : ICompanyMemberRepository
{
    private readonly Dictionary<Guid, CompanyMember> members = [];

    public IReadOnlyCollection<CompanyMember> Members => members.Values.ToArray();

    public Task<CompanyMember?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        members.TryGetValue(id, out var member);

        return Task.FromResult(member);
    }

    public Task<IReadOnlyCollection<CompanyMember>> GetActiveMembersByCompanyIdAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<CompanyMember> result = members.Values
            .Where(member => member.CompanyId == companyId && member.IsActive)
            .ToArray();

        return Task.FromResult(result);
    }

    public Task<IReadOnlyCollection<CompanyMember>> GetActiveMembershipsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<CompanyMember> result = members.Values
            .Where(member => member.UserId == userId && member.IsActive)
            .ToArray();

        return Task.FromResult(result);
    }

    public Task<CompanyMember?> GetActiveMembershipAsync(
        Guid companyId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var membership = members.Values.FirstOrDefault(member =>
            member.CompanyId == companyId
            && member.UserId == userId
            && member.IsActive);

        return Task.FromResult(membership);
    }

    public Task AddAsync(CompanyMember entity, CancellationToken cancellationToken = default)
    {
        members[entity.Id] = entity;

        return Task.CompletedTask;
    }
}

internal sealed class InMemoryResourceReservationRepository : IResourceReservationRepository
{
    private readonly Dictionary<Guid, ResourceReservation> reservations = [];

    public IReadOnlyCollection<ResourceReservation> Reservations => reservations.Values.ToArray();

    public Task<ResourceReservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        reservations.TryGetValue(id, out var reservation);

        return Task.FromResult(reservation);
    }

    public Task<IReadOnlyCollection<ResourceReservation>> GetActiveByDriverUserIdAsync(
        Guid driverUserId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<ResourceReservation> result = reservations.Values
            .Where(reservation =>
                reservation.Status == ResourceReservationStatus.Active
                && reservation.UsesDriver(driverUserId))
            .ToArray();

        return Task.FromResult(result);
    }

    public Task<IReadOnlyCollection<ResourceReservation>> GetActiveByVehicleIdAsync(
        Guid vehicleId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<ResourceReservation> result = reservations.Values
            .Where(reservation =>
                reservation.Status == ResourceReservationStatus.Active
                && reservation.UsesVehicle(vehicleId))
            .ToArray();

        return Task.FromResult(result);
    }

    public Task<IReadOnlyCollection<ResourceReservation>> GetActiveByShipmentOrderIdAsync(
        Guid shipmentOrderId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<ResourceReservation> result = reservations.Values
            .Where(reservation =>
                reservation.Status == ResourceReservationStatus.Active
                && reservation.ShipmentOrderId == shipmentOrderId)
            .ToArray();

        return Task.FromResult(result);
    }

    public Task AddAsync(ResourceReservation entity, CancellationToken cancellationToken = default)
    {
        reservations[entity.Id] = entity;

        return Task.CompletedTask;
    }
}

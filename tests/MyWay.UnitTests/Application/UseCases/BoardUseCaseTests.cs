using MyWay.Application.Common;
using MyWay.Application.Common.Pagination;
using MyWay.Application.UseCases.Boards;
using MyWay.Core.CarrierListings;
using MyWay.Core.Common.ValueObjects;
using MyWay.Core.Shipments;
using MyWay.UnitTests.Common;
using MyWay.UnitTests.TestsSupport.Fakes;

namespace MyWay.UnitTests.Application.UseCases;

public sealed class BoardUseCaseTests
{
    [Fact]
    public async Task SearchShipmentRequestBoard_ShouldReturnOnlyPublishedRequests()
    {
        var repository = new InMemoryShipmentRequestRepository();
        var publishedRequest = CreatePublishedRequest("Pickup", "Delivery");
        await repository.AddAsync(publishedRequest);
        await repository.AddAsync(CreateDraftRequest("DraftPickup", "DraftDelivery"));

        var useCase = new SearchShipmentRequestBoardUseCase(repository);

        var result = await useCase.ExecuteAsync(new SearchShipmentRequestBoardQuery(
            EmptyShipmentRequestFilter(),
            new PaginationRequest(1, 10)));

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value.Items);
        Assert.Equal(publishedRequest.Id, item.Id);
        Assert.Equal(ShipmentRequestStatus.Published.ToString(), item.Status);
    }

    [Fact]
    public async Task SearchShipmentRequestBoard_ShouldNotReturnDraftCancelledOrConvertedRequests()
    {
        var repository = new InMemoryShipmentRequestRepository();
        var publishedRequest = CreatePublishedRequest("Pickup", "Delivery");
        await repository.AddAsync(publishedRequest);
        await repository.AddAsync(CreateDraftRequest("DraftPickup", "DraftDelivery"));
        await repository.AddAsync(CreateCancelledRequest("CancelledPickup", "CancelledDelivery"));
        await repository.AddAsync(CreateConvertedRequest("ConvertedPickup", "ConvertedDelivery"));

        var useCase = new SearchShipmentRequestBoardUseCase(repository);

        var result = await useCase.ExecuteAsync(new SearchShipmentRequestBoardQuery(
            EmptyShipmentRequestFilter(),
            new PaginationRequest(1, 10)));

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value.Items);
        Assert.Equal(publishedRequest.Id, item.Id);
        Assert.Equal(1, result.Value.TotalCount);
    }

    [Fact]
    public async Task SearchShipmentRequestBoard_ShouldApplyPagination()
    {
        var repository = new InMemoryShipmentRequestRepository();
        var firstRequest = CreatePublishedRequest("FirstPickup", "FirstDelivery", DomainTestData.CreatedAt);
        var secondRequest = CreatePublishedRequest("SecondPickup", "SecondDelivery", DomainTestData.CreatedAt.AddMinutes(1));
        var thirdRequest = CreatePublishedRequest("ThirdPickup", "ThirdDelivery", DomainTestData.CreatedAt.AddMinutes(2));
        await repository.AddAsync(firstRequest);
        await repository.AddAsync(secondRequest);
        await repository.AddAsync(thirdRequest);

        var useCase = new SearchShipmentRequestBoardUseCase(repository);

        var result = await useCase.ExecuteAsync(new SearchShipmentRequestBoardQuery(
            EmptyShipmentRequestFilter(),
            new PaginationRequest(2, 1)));

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value.Items);
        Assert.Equal(secondRequest.Id, item.Id);
        Assert.Equal(2, result.Value.PageNumber);
        Assert.Equal(1, result.Value.PageSize);
        Assert.Equal(3, result.Value.TotalCount);
        Assert.Equal(3, result.Value.TotalPages);
    }

    [Fact]
    public async Task SearchShipmentRequestBoard_ShouldMapMainDtoFields()
    {
        var repository = new InMemoryShipmentRequestRepository();
        var request = CreatePublishedRequest("Pickup", "Delivery");
        await repository.AddAsync(request);

        var useCase = new SearchShipmentRequestBoardUseCase(repository);

        var result = await useCase.ExecuteAsync(new SearchShipmentRequestBoardQuery(
            EmptyShipmentRequestFilter(),
            new PaginationRequest(1, 10)));

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value.Items);
        Assert.Equal(request.Id, item.Id);
        Assert.Equal(request.CreatedByUserId, item.CreatedByUserId);
        Assert.Equal(request.CustomerUserId, item.CustomerUserId);
        Assert.Equal("Yekaterinburg", item.PickupCity);
        Assert.Equal("Yekaterinburg", item.DeliveryCity);
        Assert.Equal(request.PlannedPickupAt, item.PlannedPickupAt);
        Assert.Equal(request.CustomerPrice.Amount, item.CustomerPriceAmount);
        Assert.Equal(request.CustomerPrice.Currency, item.CustomerPriceCurrency);
        Assert.Equal(request.PublishedAt, item.PublishedAt);
    }

    [Fact]
    public async Task SearchCarrierListingBoard_ShouldReturnOnlyVisibleListings()
    {
        var repository = new InMemoryCarrierListingRepository();
        var visibleListing = CreateVisibleListing("Visible listing");
        await repository.AddAsync(visibleListing);
        await repository.AddAsync(CreateDraftListing("Draft listing"));

        var useCase = new SearchCarrierListingBoardUseCase(repository);

        var result = await useCase.ExecuteAsync(new SearchCarrierListingBoardQuery(
            EmptyCarrierListingFilter(),
            new PaginationRequest(1, 10)));

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value.Items);
        Assert.Equal(visibleListing.Id, item.Id);
        Assert.True(item.IsVisibleOnCarrierBoard);
    }

    [Fact]
    public async Task SearchCarrierListingBoard_ShouldNotReturnHiddenDraftOrInactiveListings()
    {
        var repository = new InMemoryCarrierListingRepository();
        var visibleListing = CreateVisibleListing("Visible listing");
        await repository.AddAsync(visibleListing);
        await repository.AddAsync(CreateDraftListing("Draft listing"));
        await repository.AddAsync(CreateBusyListing("Busy listing"));
        await repository.AddAsync(CreateInactiveListing("Inactive listing"));

        var useCase = new SearchCarrierListingBoardUseCase(repository);

        var result = await useCase.ExecuteAsync(new SearchCarrierListingBoardQuery(
            EmptyCarrierListingFilter(),
            new PaginationRequest(1, 10)));

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value.Items);
        Assert.Equal(visibleListing.Id, item.Id);
        Assert.Equal(1, result.Value.TotalCount);
    }

    [Fact]
    public async Task SearchCarrierListingBoard_ShouldApplyPagination()
    {
        var repository = new InMemoryCarrierListingRepository();
        var firstListing = CreateVisibleListing("First listing", DomainTestData.CreatedAt);
        var secondListing = CreateVisibleListing("Second listing", DomainTestData.CreatedAt.AddMinutes(1));
        var thirdListing = CreateVisibleListing("Third listing", DomainTestData.CreatedAt.AddMinutes(2));
        await repository.AddAsync(firstListing);
        await repository.AddAsync(secondListing);
        await repository.AddAsync(thirdListing);

        var useCase = new SearchCarrierListingBoardUseCase(repository);

        var result = await useCase.ExecuteAsync(new SearchCarrierListingBoardQuery(
            EmptyCarrierListingFilter(),
            new PaginationRequest(2, 1)));

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value.Items);
        Assert.Equal(secondListing.Id, item.Id);
        Assert.Equal(2, result.Value.PageNumber);
        Assert.Equal(1, result.Value.PageSize);
        Assert.Equal(3, result.Value.TotalCount);
        Assert.Equal(3, result.Value.TotalPages);
    }

    [Fact]
    public async Task SearchCarrierListingBoard_ShouldMapMainDtoFields()
    {
        var repository = new InMemoryCarrierListingRepository();
        var listing = CreateVisibleListing("Visible listing");
        await repository.AddAsync(listing);

        var useCase = new SearchCarrierListingBoardUseCase(repository);

        var result = await useCase.ExecuteAsync(new SearchCarrierListingBoardQuery(
            EmptyCarrierListingFilter(),
            new PaginationRequest(1, 10)));

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value.Items);
        Assert.Equal(listing.Id, item.Id);
        Assert.Equal(listing.CarrierUserId, item.CarrierUserId);
        Assert.Equal(listing.Title, item.Title);
        Assert.Equal(listing.Status.ToString(), item.Status);
        Assert.Equal(listing.BasePrice?.Amount, item.PriceAmount);
        Assert.Equal(listing.BasePrice?.Currency, item.PriceCurrency);
        Assert.Equal(listing.CreatedAt, item.CreatedAt);
        Assert.Equal(listing.PublishedAt, item.AvailableAt);
    }

    [Fact]
    public async Task GetShipmentRequestDetails_ShouldReturnDetailsForPublishedRequest()
    {
        var repository = new InMemoryShipmentRequestRepository();
        var request = CreatePublishedRequest("Pickup", "Delivery");
        await repository.AddAsync(request);
        var useCase = new GetShipmentRequestDetailsUseCase(repository);

        var result = await useCase.ExecuteAsync(new GetShipmentRequestDetailsQuery(request.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(request.Id, result.Value.Id);
        Assert.Equal(request.Type.ToString(), result.Value.Type);
        Assert.Equal(request.Status.ToString(), result.Value.Status);
        Assert.Equal("Yekaterinburg", result.Value.PickupCity);
        Assert.Contains("Pickup", result.Value.PickupAddressLine, StringComparison.Ordinal);
        Assert.Contains("Delivery", result.Value.DeliveryAddressLine, StringComparison.Ordinal);
        Assert.Equal(request.CargoDetails.Name, result.Value.CargoName);
        Assert.Equal(request.CargoDetails.WeightKg, result.Value.CargoWeightKg);
        Assert.Equal(request.CargoDetails.VolumeM3, result.Value.CargoVolumeCubicMeters);
        Assert.Equal(request.CustomerPrice.Amount, result.Value.CustomerPriceAmount);
        Assert.Equal(request.CustomerPrice.Currency, result.Value.CustomerPriceCurrency);
        Assert.Equal(request.PublishedAt, result.Value.PublishedAt);
    }

    [Fact]
    public async Task GetShipmentRequestDetails_ShouldReturnNotFound_WhenRequestDoesNotExist()
    {
        var repository = new InMemoryShipmentRequestRepository();
        var useCase = new GetShipmentRequestDetailsUseCase(repository);

        var result = await useCase.ExecuteAsync(new GetShipmentRequestDetailsQuery(Guid.NewGuid()));

        AssertFailure(result, "ShipmentRequests.NotFound");
    }

    [Fact]
    public async Task GetShipmentRequestDetails_ShouldReturnNotPublished_WhenRequestIsNotPublished()
    {
        var repository = new InMemoryShipmentRequestRepository();
        var request = CreateDraftRequest("Pickup", "Delivery");
        await repository.AddAsync(request);
        var useCase = new GetShipmentRequestDetailsUseCase(repository);

        var result = await useCase.ExecuteAsync(new GetShipmentRequestDetailsQuery(request.Id));

        AssertFailure(result, "ShipmentRequests.NotPublished");
    }

    [Fact]
    public async Task GetCarrierListingDetails_ShouldReturnDetailsForVisibleListing()
    {
        var repository = new InMemoryCarrierListingRepository();
        var listing = CreateVisibleListing("Visible listing");
        await repository.AddAsync(listing);
        var useCase = new GetCarrierListingDetailsUseCase(repository);

        var result = await useCase.ExecuteAsync(new GetCarrierListingDetailsQuery(listing.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(listing.Id, result.Value.Id);
        Assert.Equal(listing.CarrierUserId, result.Value.CarrierUserId);
        Assert.Equal(listing.Title, result.Value.Title);
        Assert.Equal(listing.Status.ToString(), result.Value.Status);
        Assert.True(result.Value.IsVisibleOnCarrierBoard);
        Assert.Equal(listing.BasePrice?.Amount, result.Value.PriceAmount);
        Assert.Equal(listing.BasePrice?.Currency, result.Value.PriceCurrency);
        Assert.Equal(listing.CreatedAt, result.Value.CreatedAt);
        Assert.Equal(listing.PublishedAt, result.Value.AvailableAt);
    }

    [Fact]
    public async Task GetCarrierListingDetails_ShouldReturnNotFound_WhenListingDoesNotExist()
    {
        var repository = new InMemoryCarrierListingRepository();
        var useCase = new GetCarrierListingDetailsUseCase(repository);

        var result = await useCase.ExecuteAsync(new GetCarrierListingDetailsQuery(Guid.NewGuid()));

        AssertFailure(result, "CarrierListings.NotFound");
    }

    [Fact]
    public async Task GetCarrierListingDetails_ShouldReturnNotAvailable_WhenListingIsNotVisible()
    {
        var repository = new InMemoryCarrierListingRepository();
        var listing = CreateDraftListing("Draft listing");
        await repository.AddAsync(listing);
        var useCase = new GetCarrierListingDetailsUseCase(repository);

        var result = await useCase.ExecuteAsync(new GetCarrierListingDetailsQuery(listing.Id));

        AssertFailure(result, "CarrierListings.NotAvailable");
    }

    private static ShipmentRequestBoardFilter EmptyShipmentRequestFilter()
    {
        return new ShipmentRequestBoardFilter(
            null,
            null,
            null,
            null,
            null,
            null,
            null);
    }

    private static CarrierListingBoardFilter EmptyCarrierListingFilter()
    {
        return new CarrierListingBoardFilter(
            null,
            null,
            null,
            null,
            null,
            null);
    }

    private static ShipmentRequest CreateDraftRequest(
        string pickupStreet,
        string deliveryStreet,
        DateTimeOffset? createdAt = null)
    {
        return ShipmentRequest.CreatePublic(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            DomainTestData.CreateAddress(pickupStreet),
            DomainTestData.CreateAddress(deliveryStreet),
            DomainTestData.CreateCargo(),
            DomainTestData.CreateMoney(),
            DomainTestData.PlannedPickupAt,
            DomainTestData.PlannedDeliveryAt,
            createdAt ?? DomainTestData.CreatedAt);
    }

    private static ShipmentRequest CreatePublishedRequest(
        string pickupStreet,
        string deliveryStreet,
        DateTimeOffset? createdAt = null)
    {
        var request = CreateDraftRequest(pickupStreet, deliveryStreet, createdAt);

        request.Publish(DomainTestData.ChangedAt);

        return request;
    }

    private static ShipmentRequest CreateCancelledRequest(string pickupStreet, string deliveryStreet)
    {
        var request = CreateDraftRequest(pickupStreet, deliveryStreet);

        request.Cancel("Cancelled", DomainTestData.ChangedAt);

        return request;
    }

    private static ShipmentRequest CreateConvertedRequest(string pickupStreet, string deliveryStreet)
    {
        var request = CreatePublishedRequest(pickupStreet, deliveryStreet);

        request.AcceptOffer(Guid.NewGuid());
        request.MarkConvertedToOrder();

        return request;
    }

    private static CarrierListing CreateDraftListing(
        string title,
        DateTimeOffset? createdAt = null)
    {
        return CarrierListing.CreateForUserCarrier(
            Guid.NewGuid(),
            Guid.NewGuid(),
            title,
            createdAt ?? DomainTestData.CreatedAt,
            basePrice: new Money(12_000, "RUB"));
    }

    private static CarrierListing CreateVisibleListing(
        string title,
        DateTimeOffset? createdAt = null)
    {
        var listing = CreateDraftListing(title, createdAt);

        listing.SetAvailable(DomainTestData.ChangedAt);

        return listing;
    }

    private static CarrierListing CreateBusyListing(string title)
    {
        var listing = CreateVisibleListing(title);

        listing.SetBusy(DomainTestData.ChangedAt);

        return listing;
    }

    private static CarrierListing CreateInactiveListing(string title)
    {
        var listing = CreateVisibleListing(title);

        listing.Deactivate(DomainTestData.ChangedAt);

        return listing;
    }

    private static void AssertFailure<T>(Result<T> result, string expectedCode)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(expectedCode, result.Error?.Code);
    }
}

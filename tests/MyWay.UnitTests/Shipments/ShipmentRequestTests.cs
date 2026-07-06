using MyWay.Core.Common.Exceptions;
using MyWay.Core.Shipments;
using MyWay.UnitTests.Common;

namespace MyWay.UnitTests.Shipments;

public sealed class ShipmentRequestTests
{
    [Fact]
    public void CreatePublic_ShouldCreateDraftRequest()
    {
        var request = CreatePublicRequest();

        Assert.Equal(ShipmentRequestType.Public, request.Type);
        Assert.Equal(ShipmentRequestStatus.Draft, request.Status);
        Assert.Null(request.TargetCarrierListingId);
    }

    [Fact]
    public void CreateDirectToCarrier_ShouldCreateDraftRequestWithTargetCarrierListing()
    {
        var targetCarrierListingId = Guid.NewGuid();

        var request = ShipmentRequest.CreateDirectToCarrier(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            targetCarrierListingId,
            DomainTestData.CreateAddress("Pickup"),
            DomainTestData.CreateAddress("Delivery"),
            DomainTestData.CreateCargo(),
            DomainTestData.CreateMoney(),
            DomainTestData.PlannedPickupAt,
            DomainTestData.PlannedDeliveryAt,
            DomainTestData.CreatedAt);

        Assert.Equal(ShipmentRequestType.DirectToCarrier, request.Type);
        Assert.Equal(ShipmentRequestStatus.Draft, request.Status);
        Assert.Equal(targetCarrierListingId, request.TargetCarrierListingId);
    }

    [Fact]
    public void CreatePublic_ShouldThrow_WhenCustomerIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => ShipmentRequest.CreatePublic(
                Guid.NewGuid(),
                null,
                null,
                DomainTestData.CreateAddress("Pickup"),
                DomainTestData.CreateAddress("Delivery"),
                DomainTestData.CreateCargo(),
                DomainTestData.CreateMoney(),
                DomainTestData.PlannedPickupAt,
                DomainTestData.PlannedDeliveryAt,
                DomainTestData.CreatedAt));

        Assert.Contains("customer", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreatePublic_ShouldThrow_WhenUserAndCompanyCustomersAreProvided()
    {
        var exception = Assert.Throws<DomainException>(
            () => ShipmentRequest.CreatePublic(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DomainTestData.CreateAddress("Pickup"),
                DomainTestData.CreateAddress("Delivery"),
                DomainTestData.CreateCargo(),
                DomainTestData.CreateMoney(),
                DomainTestData.PlannedPickupAt,
                DomainTestData.PlannedDeliveryAt,
                DomainTestData.CreatedAt));

        Assert.Contains("both", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreatePublic_ShouldThrow_WhenTargetCarrierListingIsProvided()
    {
        var exception = Assert.Throws<DomainException>(
            () => ShipmentRequest.CreatePublic(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                DomainTestData.CreateAddress("Pickup"),
                DomainTestData.CreateAddress("Delivery"),
                DomainTestData.CreateCargo(),
                DomainTestData.CreateMoney(),
                DomainTestData.PlannedPickupAt,
                DomainTestData.PlannedDeliveryAt,
                DomainTestData.CreatedAt,
                targetCarrierListingId: Guid.NewGuid()));

        Assert.Contains("target", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateDirectToCarrier_ShouldThrow_WhenTargetCarrierListingIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => ShipmentRequest.CreateDirectToCarrier(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                Guid.Empty,
                DomainTestData.CreateAddress("Pickup"),
                DomainTestData.CreateAddress("Delivery"),
                DomainTestData.CreateCargo(),
                DomainTestData.CreateMoney(),
                DomainTestData.PlannedPickupAt,
                DomainTestData.PlannedDeliveryAt,
                DomainTestData.CreatedAt));

        Assert.Contains("target", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Publish_ShouldMoveDraftRequestToPublished()
    {
        var request = CreatePublicRequest();

        request.Publish(DomainTestData.ChangedAt);

        Assert.Equal(ShipmentRequestStatus.Published, request.Status);
        Assert.Equal(DomainTestData.ChangedAt, request.PublishedAt);
    }

    [Fact]
    public void Publish_ShouldThrow_WhenRequestIsAlreadyPublished()
    {
        var request = CreatePublicRequest();
        request.Publish(DomainTestData.ChangedAt);

        Assert.Throws<DomainException>(() => request.Publish(DomainTestData.ChangedAt));
    }

    [Fact]
    public void Cancel_ShouldWorkForDraftOrPublishedRequest()
    {
        var draftRequest = CreatePublicRequest();
        var publishedRequest = CreatePublicRequest();
        publishedRequest.Publish(DomainTestData.ChangedAt);

        draftRequest.Cancel("Not needed", DomainTestData.ChangedAt);
        publishedRequest.Cancel(null, DomainTestData.ChangedAt);

        Assert.Equal(ShipmentRequestStatus.Cancelled, draftRequest.Status);
        Assert.Equal(ShipmentRequestStatus.Cancelled, publishedRequest.Status);
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenRequestIsConvertedToOrder()
    {
        var request = CreatePublicRequest();
        request.Publish(DomainTestData.ChangedAt);
        request.AcceptOffer(Guid.NewGuid());
        request.MarkConvertedToOrder();

        Assert.Throws<DomainException>(() => request.Cancel(null, DomainTestData.ChangedAt));
    }

    [Fact]
    public void AcceptOffer_ShouldWorkOnlyForPublishedRequest()
    {
        var draftRequest = CreatePublicRequest();
        var publishedRequest = CreatePublicRequest();
        var offerId = Guid.NewGuid();

        Assert.Throws<DomainException>(() => draftRequest.AcceptOffer(offerId));

        publishedRequest.Publish(DomainTestData.ChangedAt);
        publishedRequest.AcceptOffer(offerId);

        Assert.Equal(ShipmentRequestStatus.OfferAccepted, publishedRequest.Status);
        Assert.Equal(offerId, publishedRequest.AcceptedOfferId);
    }

    private static ShipmentRequest CreatePublicRequest()
    {
        return ShipmentRequest.CreatePublic(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            DomainTestData.CreateAddress("Pickup"),
            DomainTestData.CreateAddress("Delivery"),
            DomainTestData.CreateCargo(),
            DomainTestData.CreateMoney(),
            DomainTestData.PlannedPickupAt,
            DomainTestData.PlannedDeliveryAt,
            DomainTestData.CreatedAt);
    }
}

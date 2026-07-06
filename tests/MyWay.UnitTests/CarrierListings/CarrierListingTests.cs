using MyWay.Core.CarrierListings;
using MyWay.Core.Common.Exceptions;
using MyWay.UnitTests.Common;

namespace MyWay.UnitTests.CarrierListings;

public sealed class CarrierListingTests
{
    [Fact]
    public void CreateForUserCarrier_ShouldCreateDraftListing()
    {
        var carrierUserId = Guid.NewGuid();

        var listing = CarrierListing.CreateForUserCarrier(
            Guid.NewGuid(),
            carrierUserId,
            "Available van",
            DomainTestData.CreatedAt);

        Assert.Equal(CarrierListingStatus.Draft, listing.Status);
        Assert.False(listing.IsVisibleOnCarrierBoard);
        Assert.Equal(carrierUserId, listing.CarrierUserId);
        Assert.Null(listing.CarrierCompanyId);
    }

    [Fact]
    public void CreateForCompanyCarrier_ShouldCreateDraftListing()
    {
        var carrierCompanyId = Guid.NewGuid();

        var listing = CarrierListing.CreateForCompanyCarrier(
            Guid.NewGuid(),
            carrierCompanyId,
            "Company truck",
            DomainTestData.CreatedAt);

        Assert.Equal(CarrierListingStatus.Draft, listing.Status);
        Assert.False(listing.IsVisibleOnCarrierBoard);
        Assert.Null(listing.CarrierUserId);
        Assert.Equal(carrierCompanyId, listing.CarrierCompanyId);
    }

    [Fact]
    public void SetAvailable_ShouldMakeListingVisible()
    {
        var listing = CreateUserListing();

        listing.SetAvailable(DomainTestData.ChangedAt);

        Assert.Equal(CarrierListingStatus.Available, listing.Status);
        Assert.True(listing.IsVisibleOnCarrierBoard);
        Assert.Equal(DomainTestData.ChangedAt, listing.PublishedAt);
    }

    [Fact]
    public void SetBusy_ShouldMakeListingInvisible()
    {
        var listing = CreateUserListing();
        listing.SetAvailable(DomainTestData.ChangedAt);

        listing.SetBusy(DomainTestData.ChangedAt);

        Assert.Equal(CarrierListingStatus.Busy, listing.Status);
        Assert.False(listing.IsVisibleOnCarrierBoard);
    }

    [Fact]
    public void Deactivate_ShouldMakeListingInvisible()
    {
        var listing = CreateUserListing();
        listing.SetAvailable(DomainTestData.ChangedAt);

        listing.Deactivate(DomainTestData.ChangedAt);

        Assert.Equal(CarrierListingStatus.Inactive, listing.Status);
        Assert.False(listing.IsVisibleOnCarrierBoard);
    }

    [Fact]
    public void Create_ShouldThrow_WhenCarrierIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => CarrierListing.Create(
                Guid.NewGuid(),
                null,
                null,
                "Listing",
                DomainTestData.CreatedAt));

        Assert.Contains("carrier", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateForUserCarrier_ShouldThrow_WhenTitleIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => CarrierListing.CreateForUserCarrier(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "",
                DomainTestData.CreatedAt));

        Assert.Contains("title", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static CarrierListing CreateUserListing()
    {
        return CarrierListing.CreateForUserCarrier(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Available van",
            DomainTestData.CreatedAt);
    }
}

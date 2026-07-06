using MyWay.Core.Common.Exceptions;
using MyWay.Core.Shipments;
using MyWay.UnitTests.Common;

namespace MyWay.UnitTests.Shipments;

public sealed class ShipmentOfferTests
{
    [Fact]
    public void CreateForUserCarrier_ShouldCreateSubmittedOffer()
    {
        var carrierUserId = Guid.NewGuid();

        var offer = ShipmentOffer.CreateForUserCarrier(
            Guid.NewGuid(),
            carrierUserId,
            Guid.NewGuid(),
            DomainTestData.CreateMoney(),
            DomainTestData.CreatedAt);

        Assert.Equal(ShipmentOfferStatus.Submitted, offer.Status);
        Assert.Equal(carrierUserId, offer.CarrierUserId);
        Assert.Null(offer.CarrierCompanyId);
    }

    [Fact]
    public void CreateForCompanyCarrier_ShouldCreateSubmittedOffer()
    {
        var carrierCompanyId = Guid.NewGuid();

        var offer = ShipmentOffer.CreateForCompanyCarrier(
            Guid.NewGuid(),
            carrierCompanyId,
            Guid.NewGuid(),
            DomainTestData.CreateMoney(),
            DomainTestData.CreatedAt);

        Assert.Equal(ShipmentOfferStatus.Submitted, offer.Status);
        Assert.Null(offer.CarrierUserId);
        Assert.Equal(carrierCompanyId, offer.CarrierCompanyId);
    }

    [Fact]
    public void Create_ShouldThrow_WhenCarrierIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => ShipmentOffer.Create(
                Guid.NewGuid(),
                null,
                null,
                Guid.NewGuid(),
                DomainTestData.CreateMoney(),
                DomainTestData.CreatedAt));

        Assert.Contains("carrier", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenUserAndCompanyCarriersAreProvided()
    {
        var exception = Assert.Throws<DomainException>(
            () => ShipmentOffer.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DomainTestData.CreateMoney(),
                DomainTestData.CreatedAt));

        Assert.Contains("both", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Accept_ShouldWorkOnlyForSubmittedOffer()
    {
        var offer = CreateUserCarrierOffer();

        offer.Accept(DomainTestData.ChangedAt);

        Assert.Equal(ShipmentOfferStatus.Accepted, offer.Status);
        Assert.Throws<DomainException>(() => offer.Accept(DomainTestData.ChangedAt));
    }

    [Fact]
    public void Reject_ShouldWorkOnlyForSubmittedOffer()
    {
        var offer = CreateUserCarrierOffer();

        offer.Reject(DomainTestData.ChangedAt);

        Assert.Equal(ShipmentOfferStatus.Rejected, offer.Status);
        Assert.Throws<DomainException>(() => offer.Reject(DomainTestData.ChangedAt));
    }

    [Fact]
    public void Withdraw_ShouldWorkOnlyForSubmittedOffer()
    {
        var offer = CreateUserCarrierOffer();

        offer.Withdraw(DomainTestData.ChangedAt);

        Assert.Equal(ShipmentOfferStatus.Withdrawn, offer.Status);
        Assert.Throws<DomainException>(() => offer.Withdraw(DomainTestData.ChangedAt));
    }

    private static ShipmentOffer CreateUserCarrierOffer()
    {
        return ShipmentOffer.CreateForUserCarrier(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DomainTestData.CreateMoney(),
            DomainTestData.CreatedAt);
    }
}

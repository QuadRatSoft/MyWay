using MyWay.Core.Common.Exceptions;
using MyWay.Core.Reviews;
using MyWay.UnitTests.Common;

namespace MyWay.UnitTests.Reviews;

public sealed class ReviewTests
{
    [Fact]
    public void CreateForCarrierProfile_ShouldCreateReview()
    {
        var targetId = Guid.NewGuid();

        var review = Review.CreateForCarrierProfile(
            Guid.NewGuid(),
            Guid.NewGuid(),
            targetId,
            5,
            DomainTestData.CreatedAt,
            "Good carrier");

        Assert.Equal(ReviewTargetType.CarrierProfile, review.TargetType);
        Assert.Equal(targetId, review.TargetId);
        Assert.Equal(5, review.Rating);
    }

    [Fact]
    public void CreateForCustomerProfile_ShouldCreateReview()
    {
        var targetId = Guid.NewGuid();

        var review = Review.CreateForCustomerProfile(
            Guid.NewGuid(),
            Guid.NewGuid(),
            targetId,
            4,
            DomainTestData.CreatedAt);

        Assert.Equal(ReviewTargetType.CustomerProfile, review.TargetType);
        Assert.Equal(targetId, review.TargetId);
    }

    [Fact]
    public void CreateForDriverProfile_ShouldCreateReview()
    {
        var targetId = Guid.NewGuid();

        var review = Review.CreateForDriverProfile(
            Guid.NewGuid(),
            Guid.NewGuid(),
            targetId,
            5,
            DomainTestData.CreatedAt);

        Assert.Equal(ReviewTargetType.DriverProfile, review.TargetType);
        Assert.Equal(targetId, review.TargetId);
    }

    [Fact]
    public void Create_ShouldThrow_WhenShipmentOrderIdIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => Review.CreateForCarrierProfile(
                Guid.Empty,
                Guid.NewGuid(),
                Guid.NewGuid(),
                5,
                DomainTestData.CreatedAt));

        Assert.Contains("shipment order", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenFromUserIdIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => Review.CreateForCarrierProfile(
                Guid.NewGuid(),
                Guid.Empty,
                Guid.NewGuid(),
                5,
                DomainTestData.CreatedAt));

        Assert.Contains("from user", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenTargetIdIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => Review.CreateForCarrierProfile(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.Empty,
                5,
                DomainTestData.CreatedAt));

        Assert.Contains("target id", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenRatingIsLessThanOne()
    {
        var exception = Assert.Throws<DomainException>(
            () => Review.CreateForCarrierProfile(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                0,
                DomainTestData.CreatedAt));

        Assert.Contains("rating", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenRatingIsGreaterThanFive()
    {
        var exception = Assert.Throws<DomainException>(
            () => Review.CreateForCarrierProfile(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                6,
                DomainTestData.CreatedAt));

        Assert.Contains("rating", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldStoreEmptyTextAsNull()
    {
        var review = Review.CreateForCarrierProfile(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            5,
            DomainTestData.CreatedAt,
            "   ");

        Assert.Null(review.Text);
    }
}

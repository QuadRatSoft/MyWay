using MyWay.Core.Common.Exceptions;
using MyWay.Core.Common.ValueObjects;

namespace MyWay.UnitTests.Common;

public sealed class DateRangeTests
{
    [Fact]
    public void Create_ShouldThrow_WhenEndIsNotLaterThanStart()
    {
        var startAt = DomainTestData.PlannedPickupAt;

        var exception = Assert.Throws<DomainException>(() => new DateRange(startAt, startAt));

        Assert.Contains("end", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldCreateValidPeriod()
    {
        var period = new DateRange(DomainTestData.PlannedPickupAt, DomainTestData.PlannedDeliveryAt);

        Assert.Equal(DomainTestData.PlannedPickupAt, period.StartAt);
        Assert.Equal(DomainTestData.PlannedDeliveryAt, period.EndAt);
    }
}

using MyWay.Core.Common.Exceptions;
using MyWay.Core.Common.ValueObjects;

namespace MyWay.UnitTests.Common;

public sealed class ValueObjectTests
{
    [Fact]
    public void Money_ShouldThrow_WhenAmountIsNegative()
    {
        var exception = Assert.Throws<DomainException>(() => new Money(-1, "RUB"));

        Assert.Contains("amount", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Address_ShouldThrow_WhenCityIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => new Address("Russia", "Sverdlovsk Oblast", "", "Lenina", "1"));

        Assert.Contains("city", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ContactInfo_ShouldThrow_WhenPhoneIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => new ContactInfo("Logist", ""));

        Assert.Contains("phone", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}

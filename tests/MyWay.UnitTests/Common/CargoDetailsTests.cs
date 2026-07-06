using MyWay.Core.Common.Exceptions;
using MyWay.Core.Common.ValueObjects;

namespace MyWay.UnitTests.Common;

public sealed class CargoDetailsTests
{
    [Fact]
    public void Create_ShouldThrow_WhenNameIsMissing()
    {
        var exception = Assert.Throws<DomainException>(() => new CargoDetails("", 100));

        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenWeightIsNegative()
    {
        var exception = Assert.Throws<DomainException>(() => new CargoDetails("Boxes", -1));

        Assert.Contains("weight", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenVolumeIsNegative()
    {
        var exception = Assert.Throws<DomainException>(() => new CargoDetails("Boxes", 100, volumeM3: -1));

        Assert.Contains("volume", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}

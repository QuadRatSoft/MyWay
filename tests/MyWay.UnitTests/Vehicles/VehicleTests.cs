using MyWay.Core.Common.Exceptions;
using MyWay.Core.Vehicles;

namespace MyWay.UnitTests.Vehicles;

public sealed class VehicleTests
{
    [Fact]
    public void CreateForUser_ShouldCreateUserVehicle()
    {
        var ownerUserId = Guid.NewGuid();

        var vehicle = Vehicle.CreateForUser(ownerUserId, VehicleType.Van, "A123AA", 500);

        Assert.Equal(ownerUserId, vehicle.OwnerUserId);
        Assert.Null(vehicle.OwnerCompanyId);
        Assert.Equal(VehicleOwnershipType.User, vehicle.OwnershipType);
        Assert.True(vehicle.IsActive);
    }

    [Fact]
    public void CreateForCompany_ShouldCreateCompanyVehicle()
    {
        var ownerCompanyId = Guid.NewGuid();

        var vehicle = Vehicle.CreateForCompany(ownerCompanyId, VehicleType.Truck, "B456BB", 1500);

        Assert.Null(vehicle.OwnerUserId);
        Assert.Equal(ownerCompanyId, vehicle.OwnerCompanyId);
        Assert.Equal(VehicleOwnershipType.Company, vehicle.OwnershipType);
        Assert.True(vehicle.IsActive);
    }

    [Fact]
    public void Create_ShouldThrow_WhenOwnerIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => Vehicle.Create(null, null, VehicleType.Van, "A123AA", 500));

        Assert.Contains("owner", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenUserAndCompanyOwnersAreProvided()
    {
        var exception = Assert.Throws<DomainException>(
            () => Vehicle.Create(Guid.NewGuid(), Guid.NewGuid(), VehicleType.Van, "A123AA", 500));

        Assert.Contains("both", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenCapacityIsNegative()
    {
        var exception = Assert.Throws<DomainException>(
            () => Vehicle.CreateForUser(Guid.NewGuid(), VehicleType.Van, "A123AA", -1));

        Assert.Contains("capacity", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}

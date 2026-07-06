using MyWay.Core.Common.Exceptions;
using MyWay.Core.Common.ValueObjects;
using MyWay.Core.Warehouses;

namespace MyWay.UnitTests.Warehouses;

public sealed class WarehouseTests
{
    [Fact]
    public void CreateForUser_ShouldCreateUserWarehouse()
    {
        var ownerUserId = Guid.NewGuid();

        var warehouse = Warehouse.CreateForUser(ownerUserId, "User warehouse", CreateAddress());

        Assert.Equal(ownerUserId, warehouse.OwnerUserId);
        Assert.Null(warehouse.OwnerCompanyId);
        Assert.True(warehouse.IsActive);
    }

    [Fact]
    public void CreateForCompany_ShouldCreateCompanyWarehouse()
    {
        var ownerCompanyId = Guid.NewGuid();

        var warehouse = Warehouse.CreateForCompany(ownerCompanyId, "Company warehouse", CreateAddress());

        Assert.Null(warehouse.OwnerUserId);
        Assert.Equal(ownerCompanyId, warehouse.OwnerCompanyId);
        Assert.True(warehouse.IsActive);
    }

    [Fact]
    public void Create_ShouldThrow_WhenOwnerIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => Warehouse.Create(null, null, "Warehouse", CreateAddress()));

        Assert.Contains("owner", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenUserAndCompanyOwnersAreProvided()
    {
        var exception = Assert.Throws<DomainException>(
            () => Warehouse.Create(Guid.NewGuid(), Guid.NewGuid(), "Warehouse", CreateAddress()));

        Assert.Contains("both", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static Address CreateAddress()
    {
        return new Address(
            "Russia",
            "Sverdlovsk Oblast",
            "Yekaterinburg",
            "Lenina",
            "1");
    }
}

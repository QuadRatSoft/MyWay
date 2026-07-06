using MyWay.Core.Common.Exceptions;
using MyWay.Core.Common.ValueObjects;
using MyWay.Core.Warehouses;

namespace MyWay.UnitTests.Warehouses;

public sealed class WarehouseTests
{
    private static readonly DateTimeOffset CreatedAt = new(2026, 7, 6, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void CreateForUser_ShouldCreateUserWarehouse()
    {
        var ownerUserId = Guid.NewGuid();

        var warehouse = Warehouse.CreateForUser(ownerUserId, "User warehouse", CreateAddress(), CreatedAt);

        Assert.Equal(ownerUserId, warehouse.OwnerUserId);
        Assert.Null(warehouse.OwnerCompanyId);
        Assert.True(warehouse.IsActive);
        Assert.Equal(CreatedAt, warehouse.CreatedAt);
    }

    [Fact]
    public void CreateForCompany_ShouldCreateCompanyWarehouse()
    {
        var ownerCompanyId = Guid.NewGuid();

        var warehouse = Warehouse.CreateForCompany(ownerCompanyId, "Company warehouse", CreateAddress(), CreatedAt);

        Assert.Null(warehouse.OwnerUserId);
        Assert.Equal(ownerCompanyId, warehouse.OwnerCompanyId);
        Assert.True(warehouse.IsActive);
        Assert.Equal(CreatedAt, warehouse.CreatedAt);
    }

    [Fact]
    public void Create_ShouldThrow_WhenOwnerIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => Warehouse.Create(null, null, "Warehouse", CreateAddress(), CreatedAt));

        Assert.Contains("owner", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenUserAndCompanyOwnersAreProvided()
    {
        var exception = Assert.Throws<DomainException>(
            () => Warehouse.Create(Guid.NewGuid(), Guid.NewGuid(), "Warehouse", CreateAddress(), CreatedAt));

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

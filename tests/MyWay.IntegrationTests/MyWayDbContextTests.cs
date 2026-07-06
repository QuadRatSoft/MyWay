using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MyWay.Core.CarrierListings;
using MyWay.Core.Common.ValueObjects;
using MyWay.Core.Shipments;
using MyWay.Core.Users;
using MyWay.EF;

namespace MyWay.IntegrationTests;

public sealed class MyWayDbContextTests
{
    [Fact]
    public void Can_build_model_with_npgsql_options()
    {
        var options = new DbContextOptionsBuilder<MyWayDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=myway;Username=myway;Password=myway")
            .Options;

        using var dbContext = new MyWayDbContext(options);
        var model = dbContext.Model;

        Assert.NotNull(model.FindEntityType(typeof(User)));
        Assert.NotNull(model.FindEntityType(typeof(ShipmentOrder)));
        Assert.NotNull(model.FindEntityType(typeof(CarrierListing)));

        AssertOptionalMoney(model.FindEntityType(typeof(ShipmentOrder))!, nameof(ShipmentOrder.PlatformCommissionAmount));
        AssertOptionalMoney(model.FindEntityType(typeof(CarrierListing))!, nameof(CarrierListing.BasePrice));
    }

    private static void AssertOptionalMoney(IEntityType ownerType, string navigationName)
    {
        var navigation = ownerType.FindNavigation(navigationName);

        Assert.NotNull(navigation);
        Assert.False(navigation.ForeignKey.IsRequiredDependent);

        var targetType = navigation.TargetEntityType;

        Assert.Equal(typeof(Money), targetType.ClrType);
        AssertOptionalColumn(targetType, nameof(Money.Amount));
        AssertOptionalColumn(targetType, nameof(Money.Currency));
    }

    private static void AssertOptionalColumn(IEntityType entityType, string propertyName)
    {
        var property = entityType.FindProperty(propertyName);
        var tableName = entityType.GetTableName();

        Assert.NotNull(property);
        Assert.NotNull(tableName);

        var storeObject = StoreObjectIdentifier.Table(tableName, entityType.GetSchema());

        Assert.True(property.IsColumnNullable(storeObject));
    }
}

using MyWay.Core.Common.ValueObjects;

namespace MyWay.UnitTests.Common;

internal static class DomainTestData
{
    public static readonly DateTimeOffset CreatedAt = new(2026, 7, 6, 10, 0, 0, TimeSpan.Zero);

    public static readonly DateTimeOffset ChangedAt = new(2026, 7, 6, 11, 0, 0, TimeSpan.Zero);

    public static readonly DateTimeOffset PlannedPickupAt = new(2026, 7, 7, 9, 0, 0, TimeSpan.Zero);

    public static readonly DateTimeOffset PlannedDeliveryAt = new(2026, 7, 7, 17, 0, 0, TimeSpan.Zero);

    public static Address CreateAddress(string street = "Lenina")
    {
        return new Address(
            "Russia",
            "Sverdlovsk Oblast",
            "Yekaterinburg",
            street,
            "1");
    }

    public static CargoDetails CreateCargo()
    {
        return new CargoDetails("Boxes", 100, volumeM3: 2);
    }

    public static Money CreateMoney()
    {
        return new Money(10_000, "RUB");
    }

    public static DateRange CreatePeriod()
    {
        return new DateRange(PlannedPickupAt, PlannedDeliveryAt);
    }
}

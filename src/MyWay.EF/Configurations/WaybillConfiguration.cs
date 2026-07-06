using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWay.Core.Waybills;

namespace MyWay.EF.Configurations;

public sealed class WaybillConfiguration : IEntityTypeConfiguration<Waybill>
{
    public void Configure(EntityTypeBuilder<Waybill> builder)
    {
        builder.ToTable("waybills");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.ShipmentOrderId)
            .HasColumnName("shipment_order_id")
            .IsRequired();

        builder.Property(x => x.Number)
            .HasColumnName("number")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.CustomerUserId)
            .HasColumnName("customer_user_id")
            .IsRequired(false);

        builder.Property(x => x.CustomerCompanyId)
            .HasColumnName("customer_company_id")
            .IsRequired(false);

        builder.Property(x => x.CarrierUserId)
            .HasColumnName("carrier_user_id")
            .IsRequired(false);

        builder.Property(x => x.CarrierCompanyId)
            .HasColumnName("carrier_company_id")
            .IsRequired(false);

        builder.Property(x => x.DriverUserId)
            .HasColumnName("driver_user_id")
            .IsRequired();

        builder.Property(x => x.VehicleId)
            .HasColumnName("vehicle_id")
            .IsRequired();

        builder.Property(x => x.DriverName)
            .HasColumnName("driver_name")
            .HasMaxLength(256)
            .IsRequired(false);

        builder.Property(x => x.DriverLicenseNumber)
            .HasColumnName("driver_license_number")
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(x => x.VehicleBrand)
            .HasColumnName("vehicle_brand")
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(x => x.VehicleModel)
            .HasColumnName("vehicle_model")
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(x => x.VehiclePlateNumber)
            .HasColumnName("vehicle_plate_number")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.OdometerStartKm)
            .HasColumnName("odometer_start_km")
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(x => x.OdometerEndKm)
            .HasColumnName("odometer_end_km")
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(x => x.Comment)
            .HasColumnName("comment")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.IssuedAt)
            .HasColumnName("issued_at")
            .IsRequired(false);

        builder.Property(x => x.ClosedAt)
            .HasColumnName("closed_at")
            .IsRequired(false);

        builder.Property(x => x.CancelledAt)
            .HasColumnName("cancelled_at")
            .IsRequired(false);

        builder.Property(x => x.CancellationReason)
            .HasColumnName("cancellation_reason")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.OwnsOne(x => x.PickupAddress, owned =>
        {
            owned.ConfigureAddress("pickup", required: true);
        });
        builder.Navigation(x => x.PickupAddress).IsRequired();

        builder.OwnsOne(x => x.DeliveryAddress, owned =>
        {
            owned.ConfigureAddress("delivery", required: true);
        });
        builder.Navigation(x => x.DeliveryAddress).IsRequired();

        builder.OwnsOne(x => x.CargoDetails, owned =>
        {
            owned.ConfigureCargoDetails("cargo");
        });
        builder.Navigation(x => x.CargoDetails).IsRequired();

        builder.OwnsOne(x => x.TripPeriod, owned =>
        {
            owned.ConfigureDateRange("period_start", "period_end");
        });
        builder.Navigation(x => x.TripPeriod).IsRequired();

        builder.HasIndex(x => new { x.ShipmentOrderId, x.Status });
    }
}

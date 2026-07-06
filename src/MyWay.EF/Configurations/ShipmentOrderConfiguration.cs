using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWay.Core.Shipments;

namespace MyWay.EF.Configurations;

public sealed class ShipmentOrderConfiguration : IEntityTypeConfiguration<ShipmentOrder>
{
    public void Configure(EntityTypeBuilder<ShipmentOrder> builder)
    {
        builder.ToTable("shipment_orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.ShipmentRequestId)
            .HasColumnName("shipment_request_id")
            .IsRequired();

        builder.Property(x => x.AcceptedOfferId)
            .HasColumnName("accepted_offer_id")
            .IsRequired();

        builder.Property(x => x.CreatedByUserId)
            .HasColumnName("created_by_user_id")
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

        builder.Property(x => x.AssignedDriverUserId)
            .HasColumnName("assigned_driver_user_id")
            .IsRequired(false);

        builder.Property(x => x.AssignedVehicleId)
            .HasColumnName("assigned_vehicle_id")
            .IsRequired(false);

        builder.Property(x => x.PlatformCommissionPercent)
            .HasColumnName("platform_commission_percent")
            .HasPrecision(5, 2)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.PlannedPickupAt)
            .HasColumnName("planned_pickup_at")
            .IsRequired();

        builder.Property(x => x.PlannedDeliveryAt)
            .HasColumnName("planned_delivery_at")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.StartedAt)
            .HasColumnName("started_at")
            .IsRequired(false);

        builder.Property(x => x.DeliveredAt)
            .HasColumnName("delivered_at")
            .IsRequired(false);

        builder.Property(x => x.CompletedAt)
            .HasColumnName("completed_at")
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

        builder.OwnsOne(x => x.FinalPrice, owned =>
        {
            owned.ConfigureMoney("final_price", required: true);
        });
        builder.Navigation(x => x.FinalPrice).IsRequired();

        builder.OwnsOne(x => x.PlatformCommissionAmount, owned =>
        {
            owned.ConfigureMoney("platform_commission", required: false);
        });
        builder.Navigation(x => x.PlatformCommissionAmount).IsRequired(false);

        builder.HasIndex(x => new
        {
            x.Status,
            x.CustomerUserId,
            x.CustomerCompanyId,
            x.CarrierUserId,
            x.CarrierCompanyId
        });
    }
}

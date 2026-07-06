using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWay.Core.Shipments;

namespace MyWay.EF.Configurations;

public sealed class ShipmentRequestConfiguration : IEntityTypeConfiguration<ShipmentRequest>
{
    public void Configure(EntityTypeBuilder<ShipmentRequest> builder)
    {
        builder.ToTable("shipment_requests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .IsRequired();

        builder.Property(x => x.CustomerUserId)
            .HasColumnName("customer_user_id")
            .IsRequired(false);

        builder.Property(x => x.CustomerCompanyId)
            .HasColumnName("customer_company_id")
            .IsRequired(false);

        builder.Property(x => x.TargetCarrierListingId)
            .HasColumnName("target_carrier_listing_id")
            .IsRequired(false);

        builder.Property(x => x.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

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

        builder.Property(x => x.PublishedAt)
            .HasColumnName("published_at")
            .IsRequired(false);

        builder.Property(x => x.CancelledAt)
            .HasColumnName("cancelled_at")
            .IsRequired(false);

        builder.Property(x => x.CancellationReason)
            .HasColumnName("cancellation_reason")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.AcceptedOfferId)
            .HasColumnName("accepted_offer_id")
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

        builder.OwnsOne(x => x.CustomerPrice, owned =>
        {
            owned.ConfigureMoney("customer_price", required: true);
        });
        builder.Navigation(x => x.CustomerPrice).IsRequired();

        builder.HasIndex(x => new { x.Status, x.Type, x.CreatedAt, x.PublishedAt });
    }
}

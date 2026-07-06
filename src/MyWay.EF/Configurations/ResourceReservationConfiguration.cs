using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWay.Core.Resources;

namespace MyWay.EF.Configurations;

public sealed class ResourceReservationConfiguration : IEntityTypeConfiguration<ResourceReservation>
{
    public void Configure(EntityTypeBuilder<ResourceReservation> builder)
    {
        builder.ToTable("resource_reservations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.ShipmentOrderId)
            .HasColumnName("shipment_order_id")
            .IsRequired();

        builder.Property(x => x.DriverUserId)
            .HasColumnName("driver_user_id")
            .IsRequired(false);

        builder.Property(x => x.VehicleId)
            .HasColumnName("vehicle_id")
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.CancelledAt)
            .HasColumnName("cancelled_at")
            .IsRequired(false);

        builder.Property(x => x.CompletedAt)
            .HasColumnName("completed_at")
            .IsRequired(false);

        builder.OwnsOne(x => x.Period, owned =>
        {
            owned.ConfigureDateRange("period_start", "period_end");
        });
        builder.Navigation(x => x.Period).IsRequired();

        builder.HasIndex(x => new { x.Status, x.DriverUserId, x.VehicleId, x.ShipmentOrderId });
    }
}

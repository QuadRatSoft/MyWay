using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWay.Core.Shipments;

namespace MyWay.EF.Configurations;

public sealed class ShipmentOfferConfiguration : IEntityTypeConfiguration<ShipmentOffer>
{
    public void Configure(EntityTypeBuilder<ShipmentOffer> builder)
    {
        builder.ToTable("shipment_offers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.ShipmentRequestId)
            .HasColumnName("shipment_request_id")
            .IsRequired();

        builder.Property(x => x.CarrierUserId)
            .HasColumnName("carrier_user_id")
            .IsRequired(false);

        builder.Property(x => x.CarrierCompanyId)
            .HasColumnName("carrier_company_id")
            .IsRequired(false);

        builder.Property(x => x.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasColumnName("comment")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.AcceptedAt)
            .HasColumnName("accepted_at")
            .IsRequired(false);

        builder.Property(x => x.RejectedAt)
            .HasColumnName("rejected_at")
            .IsRequired(false);

        builder.Property(x => x.WithdrawnAt)
            .HasColumnName("withdrawn_at")
            .IsRequired(false);

        builder.OwnsOne(x => x.OfferedPrice, owned =>
        {
            owned.ConfigureMoney("carrier_price", required: true);
        });
        builder.Navigation(x => x.OfferedPrice).IsRequired();

        builder.HasIndex(x => new { x.ShipmentRequestId, x.Status, x.CarrierUserId, x.CarrierCompanyId });
    }
}

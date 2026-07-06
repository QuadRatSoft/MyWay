using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWay.Core.Reviews;

namespace MyWay.EF.Configurations;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.ShipmentOrderId)
            .HasColumnName("shipment_order_id")
            .IsRequired();

        builder.Property(x => x.FromUserId)
            .HasColumnName("from_user_id")
            .IsRequired();

        builder.Property(x => x.TargetType)
            .HasColumnName("target_type")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.TargetId)
            .HasColumnName("target_id")
            .IsRequired();

        builder.Property(x => x.Rating)
            .HasColumnName("rating")
            .IsRequired();

        builder.Property(x => x.Text)
            .HasColumnName("text")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(x => new { x.ShipmentOrderId, x.TargetType, x.TargetId });
    }
}

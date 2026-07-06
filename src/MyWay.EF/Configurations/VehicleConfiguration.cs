using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWay.Core.Vehicles;

namespace MyWay.EF.Configurations;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.OwnerUserId)
            .HasColumnName("owner_user_id")
            .IsRequired(false);

        builder.Property(x => x.OwnerCompanyId)
            .HasColumnName("owner_company_id")
            .IsRequired(false);

        builder.Property(x => x.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.Brand)
            .HasColumnName("brand")
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(x => x.Model)
            .HasColumnName("model")
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(x => x.PlateNumber)
            .HasColumnName("plate_number")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.CapacityKg)
            .HasColumnName("capacity_kg")
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.VolumeM3)
            .HasColumnName("volume_m3")
            .HasPrecision(18, 3)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Ignore(x => x.OwnershipType);
    }
}

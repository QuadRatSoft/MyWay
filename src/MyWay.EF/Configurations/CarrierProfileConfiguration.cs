using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWay.Core.Profiles;

namespace MyWay.EF.Configurations;

public sealed class CarrierProfileConfiguration : IEntityTypeConfiguration<CarrierProfile>
{
    public void Configure(EntityTypeBuilder<CarrierProfile> builder)
    {
        builder.ToTable("carrier_profiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired(false);

        builder.Property(x => x.CompanyId)
            .HasColumnName("company_id")
            .IsRequired(false);

        builder.Property(x => x.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();
    }
}

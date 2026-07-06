using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWay.Core.Warehouses;

namespace MyWay.EF.Configurations;

public sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("warehouses");

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

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.WorkingHours)
            .HasColumnName("working_hours")
            .HasMaxLength(512)
            .IsRequired(false);

        builder.Property(x => x.DriverComment)
            .HasColumnName("driver_comment")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.OwnsOne(x => x.Address, owned =>
        {
            owned.ConfigureAddress("address", required: true);
        });
        builder.Navigation(x => x.Address).IsRequired();

        builder.OwnsOne(x => x.ContactInfo, owned =>
        {
            owned.ConfigureContactInfo("contact", required: false);
        });
        builder.Navigation(x => x.ContactInfo).IsRequired(false);
    }
}

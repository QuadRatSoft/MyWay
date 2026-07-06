using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWay.Core.Companies;

namespace MyWay.EF.Configurations;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.LegalName)
            .HasColumnName("legal_name")
            .HasMaxLength(512)
            .IsRequired(false);

        builder.Property(x => x.TaxNumber)
            .HasColumnName("tax_number")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.OwnsOne(x => x.Address, owned =>
        {
            owned.ConfigureAddress("address", required: false);
        });
        builder.Navigation(x => x.Address).IsRequired(false);

        builder.OwnsOne(x => x.ContactInfo, owned =>
        {
            owned.ConfigureContactInfo("contact", required: false);
        });
        builder.Navigation(x => x.ContactInfo).IsRequired(false);
    }
}

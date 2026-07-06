using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWay.Core.CarrierListings;

namespace MyWay.EF.Configurations;

public sealed class CarrierListingConfiguration : IEntityTypeConfiguration<CarrierListing>
{
    public void Configure(EntityTypeBuilder<CarrierListing> builder)
    {
        builder.ToTable("carrier_listings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.CarrierProfileId)
            .HasColumnName("carrier_profile_id")
            .IsRequired();

        builder.Property(x => x.CarrierUserId)
            .HasColumnName("carrier_user_id")
            .IsRequired(false);

        builder.Property(x => x.CarrierCompanyId)
            .HasColumnName("carrier_company_id")
            .IsRequired(false);

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.PublishedAt)
            .HasColumnName("published_at")
            .IsRequired(false);

        builder.Property(x => x.LastStatusChangedAt)
            .HasColumnName("last_status_changed_at")
            .IsRequired(false);

        builder.Ignore(x => x.IsVisibleOnCarrierBoard);

        builder.Property<bool>("IsVisibleOnCarrierBoardIndex")
            .HasColumnName("is_visible_on_carrier_board")
            .HasComputedColumnSql("status = 'Available'", stored: true);

        builder.OwnsOne(x => x.BasePrice, owned =>
        {
            owned.ConfigureMoney("base_price", required: false);
        });
        builder.Navigation(x => x.BasePrice).IsRequired(false);

        builder.HasIndex(
            "Status",
            "CarrierUserId",
            "CarrierCompanyId",
            "IsVisibleOnCarrierBoardIndex");
    }
}

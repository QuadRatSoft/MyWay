using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWay.Core.Common.ValueObjects;

namespace MyWay.EF.Configurations;

internal static class ValueObjectConfigurationExtensions
{
    public static void ConfigureAddress<TEntity>(
        this OwnedNavigationBuilder<TEntity, Address> builder,
        string prefix,
        bool required)
        where TEntity : class
    {
        builder.Property(x => x.Country)
            .HasColumnName($"{prefix}_country")
            .HasMaxLength(128)
            .IsRequired(required);

        builder.Property(x => x.Region)
            .HasColumnName($"{prefix}_region")
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(x => x.City)
            .HasColumnName($"{prefix}_city")
            .HasMaxLength(128)
            .IsRequired(required);

        builder.Property(x => x.Street)
            .HasColumnName($"{prefix}_street")
            .HasMaxLength(256)
            .IsRequired(required);

        builder.Property(x => x.House)
            .HasColumnName($"{prefix}_house")
            .HasMaxLength(64)
            .IsRequired(required);

        builder.Property(x => x.ApartmentOrOffice)
            .HasColumnName($"{prefix}_apartment_or_office")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.PostalCode)
            .HasColumnName($"{prefix}_postal_code")
            .HasMaxLength(32)
            .IsRequired(false);

        builder.Property(x => x.Latitude)
            .HasColumnName($"{prefix}_latitude")
            .HasPrecision(9, 6)
            .IsRequired(false);

        builder.Property(x => x.Longitude)
            .HasColumnName($"{prefix}_longitude")
            .HasPrecision(9, 6)
            .IsRequired(false);
    }

    public static void ConfigureCargoDetails<TEntity>(
        this OwnedNavigationBuilder<TEntity, CargoDetails> builder,
        string prefix)
        where TEntity : class
    {
        builder.Property(x => x.Name)
            .HasColumnName($"{prefix}_name")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName($"{prefix}_description")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.WeightKg)
            .HasColumnName($"{prefix}_weight_kg")
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.VolumeM3)
            .HasColumnName($"{prefix}_volume_m3")
            .HasPrecision(18, 3)
            .IsRequired(false);

        builder.Property(x => x.LengthCm)
            .HasColumnName($"{prefix}_length_cm")
            .HasPrecision(18, 3)
            .IsRequired(false);

        builder.Property(x => x.WidthCm)
            .HasColumnName($"{prefix}_width_cm")
            .HasPrecision(18, 3)
            .IsRequired(false);

        builder.Property(x => x.HeightCm)
            .HasColumnName($"{prefix}_height_cm")
            .HasPrecision(18, 3)
            .IsRequired(false);

        builder.Property(x => x.IsFragile)
            .HasColumnName($"{prefix}_is_fragile")
            .IsRequired();

        builder.Property(x => x.RequiresRefrigeration)
            .HasColumnName($"{prefix}_requires_refrigeration")
            .IsRequired();
    }

    public static void ConfigureContactInfo<TEntity>(
        this OwnedNavigationBuilder<TEntity, ContactInfo> builder,
        string prefix,
        bool required)
        where TEntity : class
    {
        builder.Property(x => x.Phone)
            .HasColumnName($"{prefix}_phone")
            .HasMaxLength(64)
            .IsRequired(required);

        builder.Property(x => x.Email)
            .HasColumnName($"{prefix}_email")
            .HasMaxLength(320)
            .IsRequired(false);

        builder.Property(x => x.ContactName)
            .HasColumnName($"{prefix}_person_name")
            .HasMaxLength(256)
            .IsRequired(required);
    }

    public static void ConfigureDateRange<TEntity>(
        this OwnedNavigationBuilder<TEntity, DateRange> builder,
        string startColumnName,
        string endColumnName)
        where TEntity : class
    {
        builder.Property(x => x.StartAt)
            .HasColumnName(startColumnName)
            .IsRequired();

        builder.Property(x => x.EndAt)
            .HasColumnName(endColumnName)
            .IsRequired();
    }

    public static void ConfigureMoney<TEntity>(
        this OwnedNavigationBuilder<TEntity, Money> builder,
        string prefix,
        bool required)
        where TEntity : class
    {
        var amountBuilder = builder.Property(x => x.Amount)
            .HasColumnName($"{prefix}_amount")
            .HasPrecision(18, 2);

        if (required)
        {
            amountBuilder.IsRequired();
        }

        builder.Property(x => x.Currency)
            .HasColumnName($"{prefix}_currency")
            .HasMaxLength(3)
            .IsRequired(required);
    }
}

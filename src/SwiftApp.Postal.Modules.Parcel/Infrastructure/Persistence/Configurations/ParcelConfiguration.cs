using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Parcel.Infrastructure.Persistence.Configurations;

public class ParcelConfiguration : BaseEntityConfiguration<Domain.Entities.Parcel>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Domain.Entities.Parcel> builder)
    {
        builder.ToTable("parcels");

        builder.Property(e => e.TrackingNumber).IsRequired().HasMaxLength(20).HasColumnName("tracking_number");
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50).HasColumnName("status");
        builder.Property(e => e.Type).HasConversion<string>().HasMaxLength(50).HasColumnName("type");
        builder.Property(e => e.WeightKg).HasColumnType("NUMERIC(10,3)").HasColumnName("weight_kg");
        builder.Property(e => e.LengthCm).HasColumnType("NUMERIC(10,3)").HasColumnName("length_cm");
        builder.Property(e => e.WidthCm).HasColumnType("NUMERIC(10,3)").HasColumnName("width_cm");
        builder.Property(e => e.HeightCm).HasColumnType("NUMERIC(10,3)").HasColumnName("height_cm");
        builder.Property(e => e.Price).HasColumnType("NUMERIC(19,4)").HasColumnName("price");
        builder.Property(e => e.SenderCustomerId).HasColumnName("sender_customer_id");
        builder.Property(e => e.OriginBranchId).HasColumnName("origin_branch_id");

        builder.Property(e => e.SenderName).IsRequired().HasMaxLength(200).HasColumnName("sender_name");
        builder.Property(e => e.SenderStreet).IsRequired().HasMaxLength(200).HasColumnName("sender_street");
        builder.Property(e => e.SenderZipCode).IsRequired().HasMaxLength(10).HasColumnName("sender_zip_code");
        builder.Property(e => e.SenderCity).IsRequired().HasMaxLength(100).HasColumnName("sender_city");
        builder.Property(e => e.SenderPhone).HasMaxLength(30).HasColumnName("sender_phone");

        builder.Property(e => e.RecipientName).IsRequired().HasMaxLength(200).HasColumnName("recipient_name");
        builder.Property(e => e.RecipientStreet).IsRequired().HasMaxLength(200).HasColumnName("recipient_street");
        builder.Property(e => e.RecipientZipCode).IsRequired().HasMaxLength(10).HasColumnName("recipient_zip_code");
        builder.Property(e => e.RecipientCity).IsRequired().HasMaxLength(100).HasColumnName("recipient_city");
        builder.Property(e => e.RecipientPhone).HasMaxLength(30).HasColumnName("recipient_phone");

        builder.HasIndex(e => e.TrackingNumber).IsUnique();
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.SenderCustomerId);
    }
}

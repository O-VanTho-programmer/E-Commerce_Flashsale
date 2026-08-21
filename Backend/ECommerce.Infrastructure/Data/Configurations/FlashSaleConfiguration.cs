using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class FlashSaleConfiguration : IEntityTypeConfiguration<FlashSale>
{
    public void Configure(EntityTypeBuilder<FlashSale> builder)
    {
        builder.ToTable("FlashSales");

        builder.HasKey(fs => fs.Id);

        builder.Property(fs => fs.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(fs => fs.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
    }
}

using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class FlashSaleItemConfiguration : IEntityTypeConfiguration<FlashSaleItem>
{
    public void Configure(EntityTypeBuilder<FlashSaleItem> builder)
    {
        builder.ToTable("FlashSaleItems");

        builder.HasKey(fsi => fsi.Id);

        builder.Property(fsi => fsi.SalePrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(fsi => fsi.RowVersion)
            .IsRowVersion();

        builder.HasOne(fsi => fsi.FlashSale)
            .WithMany(fs => fs.FlashSaleItems)
            .HasForeignKey(fsi => fsi.FlashSaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fsi => fsi.ProductVariant)
            .WithOne(pv => pv.FlashSaleItem)
            .HasForeignKey<FlashSaleItem>(fsi => fsi.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

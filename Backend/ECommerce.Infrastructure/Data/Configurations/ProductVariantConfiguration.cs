using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.Sku)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasIndex(pv => pv.Sku).IsUnique();

        builder.Property(pv => pv.Color)
            .HasMaxLength(50);

        builder.Property(pv => pv.Size)
            .HasMaxLength(50);

        builder.Property(pv => pv.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(pv => pv.RowVersion)
            .IsRowVersion();

        builder.HasOne(pv => pv.Product)
            .WithMany(p => p.ProductVariants)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

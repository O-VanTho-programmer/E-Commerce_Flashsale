using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ChannelStockAllocationConfiguration : IEntityTypeConfiguration<ChannelStockAllocation>
{
    public void Configure(EntityTypeBuilder<ChannelStockAllocation> builder)
    {
        builder.ToTable("ChannelStockAllocations");

        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.PlatformName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.RowVersion)
            .IsRowVersion();

        builder.HasOne(c => c.ProductVariant)
            .WithMany()
            .HasForeignKey(c => c.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ensure one allocation per platform per variant
        builder.HasIndex(c => new { c.ProductVariantId, c.PlatformName })
            .IsUnique();
    }
}

using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ExternalOrderSyncLogConfiguration : IEntityTypeConfiguration<ExternalOrderSyncLog>
{
    public void Configure(EntityTypeBuilder<ExternalOrderSyncLog> builder)
    {
        builder.ToTable("ExternalOrderSyncLogs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.PlatformName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.ExternalOrderId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50);
            
        // Idempotency guarantee
        builder.HasIndex(e => new { e.PlatformName, e.ExternalOrderId })
            .IsUnique();
    }
}

using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class WebhookLogConfiguration : IEntityTypeConfiguration<WebhookLog>
{
    public void Configure(EntityTypeBuilder<WebhookLog> builder)
    {
        builder.ToTable("WebhookLogs");

        builder.HasKey(wl => wl.Id);

        builder.Property(wl => wl.WebhookEventId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(wl => wl.WebhookEventId).IsUnique();

        builder.Property(wl => wl.ProcessStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(wl => wl.Payment)
            .WithMany(p => p.WebhookLogs)
            .HasForeignKey(wl => wl.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

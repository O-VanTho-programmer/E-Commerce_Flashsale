using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class StockReservationConfiguration : IEntityTypeConfiguration<StockReservation>
{
    public void Configure(EntityTypeBuilder<StockReservation> builder)
    {
        builder.ToTable("StockReservations");

        builder.HasKey(sr => sr.Id);

        builder.Property(sr => sr.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(sr => sr.CartItem)
            .WithOne(ci => ci.StockReservation)
            .HasForeignKey<StockReservation>(sr => sr.CartItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

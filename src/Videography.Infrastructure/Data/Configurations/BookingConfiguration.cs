using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Videography.Domain.Entities;

namespace Videography.Infrastructure.Data.Configurations;
public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);

        builder.Property(c => c.CreatedBy).HasMaxLength(20);
        builder.Property(c => c.ModifiedBy).HasMaxLength(20);

        builder.HasOne(c => c.Address)
               .WithMany(c => c.Bookings)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.CreditCard)
               .WithMany(c => c.Bookings)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
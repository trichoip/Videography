using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Videography.Domain.Entities;

namespace Videography.Infrastructure.Data.Configurations;
public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.Property(a => a.FullName).HasMaxLength(50);
        builder.Property(a => a.PhoneNumber).HasMaxLength(20);
        builder.Property(a => a.Country).HasMaxLength(50);
        builder.Property(a => a.City).HasMaxLength(50);
        builder.Property(a => a.Street).HasMaxLength(200);

    }
}
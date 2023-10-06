using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Videography.Domain.Entities;

namespace Videography.Infrastructure.Data.Configurations;
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(e => e.Name).HasMaxLength(50);
        builder.Property(c => c.CreatedBy).HasMaxLength(20);
        builder.Property(c => c.ModifiedBy).HasMaxLength(20);
    }
}
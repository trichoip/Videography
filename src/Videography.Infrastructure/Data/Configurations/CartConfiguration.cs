using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Videography.Domain.Entities;

namespace Videography.Infrastructure.Data.Configurations;
public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.Property(c => c.CreatedBy).HasMaxLength(20);
        builder.Property(c => c.ModifiedBy).HasMaxLength(20);
    }
}
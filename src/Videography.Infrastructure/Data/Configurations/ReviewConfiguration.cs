using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Videography.Domain.Entities;

namespace Videography.Infrastructure.Data.Configurations;
public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(e => e.Title).HasMaxLength(100);

        builder.Property(c => c.CreatedBy).HasMaxLength(20);
        builder.Property(c => c.ModifiedBy).HasMaxLength(20);
    }
}
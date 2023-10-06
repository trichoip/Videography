using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Videography.Domain.Entities;

namespace Videography.Infrastructure.Data.Configurations;
public class CreditCardConfiguration : IEntityTypeConfiguration<CreditCard>
{
    public void Configure(EntityTypeBuilder<CreditCard> builder)
    {
        builder.Property(c => c.CardNumber).HasMaxLength(30);
        builder.Property(c => c.CardHolderName).HasMaxLength(50);
        builder.Property(c => c.CVV).HasMaxLength(4);
        builder.Property(c => c.CreatedBy).HasMaxLength(20);
        builder.Property(c => c.ModifiedBy).HasMaxLength(20);

    }
}
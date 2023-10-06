using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Videography.Domain.Entities;

namespace Videography.Infrastructure.Data.Configurations;
public class CreditCardTypeConfiguration : IEntityTypeConfiguration<CreditCardType>
{
    public void Configure(EntityTypeBuilder<CreditCardType> builder)
    {
        builder.Property(e => e.Name).HasMaxLength(20);

    }
}
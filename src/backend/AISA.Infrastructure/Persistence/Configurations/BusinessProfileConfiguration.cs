using AISA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISA.Infrastructure.Persistence.Configurations;

public class BusinessProfileConfiguration : IEntityTypeConfiguration<BusinessProfile>
{
    public void Configure(EntityTypeBuilder<BusinessProfile> builder)
    {
        builder.HasKey(bp => bp.Id);

        builder.Property(bp => bp.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(bp => bp.Description)
            .HasMaxLength(1000);

        builder.Property(bp => bp.Category)
            .HasMaxLength(100);

        builder.Property(bp => bp.Address)
            .HasMaxLength(500);

        builder.Property(bp => bp.SubscriptionTier)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}

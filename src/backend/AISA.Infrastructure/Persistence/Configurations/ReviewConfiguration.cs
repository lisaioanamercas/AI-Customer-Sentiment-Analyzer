using AISA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISA.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Content)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(r => r.AuthorName)
            .HasMaxLength(200);

        builder.Property(r => r.Source)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(r => r.BusinessProfile)
            .WithMany(bp => bp.Reviews)
            .HasForeignKey(r => r.BusinessProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.SentimentResult)
            .WithOne(sr => sr.Review)
            .HasForeignKey<SentimentResult>(sr => sr.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.BusinessProfileId);
        builder.HasIndex(r => r.CreatedAt);
    }
}

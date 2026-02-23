using AISA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISA.Infrastructure.Persistence.Configurations;

public class SentimentResultConfiguration : IEntityTypeConfiguration<SentimentResult>
{
    public void Configure(EntityTypeBuilder<SentimentResult> builder)
    {
        builder.HasKey(sr => sr.Id);

        builder.Property(sr => sr.Label)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(sr => sr.Score)
            .IsRequired();

        builder.Property(sr => sr.ModelVersion)
            .HasMaxLength(100);

        builder.HasIndex(sr => sr.ReviewId)
            .IsUnique();
    }
}

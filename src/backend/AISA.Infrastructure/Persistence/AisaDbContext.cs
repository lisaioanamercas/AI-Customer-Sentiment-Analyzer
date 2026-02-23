using AISA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AISA.Infrastructure.Persistence;

/// <summary>
/// DbContext principal al aplicației AISA.
/// Configurează toate entitățile și relațiile.
/// </summary>
public class AisaDbContext : DbContext
{
    public AisaDbContext(DbContextOptions<AisaDbContext> options) : base(options) { }

    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<BusinessProfile> BusinessProfiles => Set<BusinessProfile>();
    public DbSet<SentimentResult> SentimentResults => Set<SentimentResult>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplică toate configurațiile din assembly-ul curent
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AisaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-setare UpdatedAt la modificări
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

using AISA.Domain.Entities;
using AISA.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AISA.Infrastructure.Persistence.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AisaDbContext _context;

    public ReviewRepository(AisaDbContext context)
    {
        _context = context;
    }

    public async Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Include(r => r.SentimentResult)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Review>> GetByBusinessProfileIdAsync(Guid businessProfileId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Include(r => r.SentimentResult)
            .Where(r => r.BusinessProfileId == businessProfileId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Review>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Include(r => r.SentimentResult)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Review> AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        await _context.Reviews.AddAsync(review, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return review;
    }

    public async Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
        _context.Reviews.Update(review);

        // EF Core marks ALL graph entities as Modified when Update() is called.
        // For a brand-new SentimentResult (not yet in DB), we must override to Added
        // so EF emits INSERT instead of UPDATE (which would silently affect 0 rows).
        if (review.SentimentResult is not null)
        {
            var entry = _context.Entry(review.SentimentResult);
            bool existsInDb = await _context.SentimentResults
                .AnyAsync(s => s.Id == review.SentimentResult.Id, cancellationToken);

            entry.State = existsInDb ? EntityState.Modified : EntityState.Added;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Must include SentimentResult so EF removes the FK child before the parent row.
        var review = await _context.Reviews
            .Include(r => r.SentimentResult)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (review is not null)
        {
            if (review.SentimentResult is not null)
                _context.SentimentResults.Remove(review.SentimentResult);

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> GetReviewCountForMonthAsync(Guid businessProfileId, DateTime month, CancellationToken cancellationToken = default)
    {
        var startOfMonth = new DateTime(month.Year, month.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endOfMonth = startOfMonth.AddMonths(1);

        return await _context.Reviews
            .CountAsync(r => r.BusinessProfileId == businessProfileId
                && r.CreatedAt >= startOfMonth
                && r.CreatedAt < endOfMonth, cancellationToken);
    }

    public async Task<IReadOnlyList<Review>> GetUnanalyzedAsync(Guid businessProfileId, int maxCount, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Include(r => r.SentimentResult)
            .Where(r => r.BusinessProfileId == businessProfileId && r.SentimentResult == null)
            .OrderByDescending(r => r.CreatedAt)
            .Take(maxCount)
            .ToListAsync(cancellationToken);
    }
}

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
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var review = await _context.Reviews.FindAsync(new object[] { id }, cancellationToken);
        if (review is not null)
        {
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
}

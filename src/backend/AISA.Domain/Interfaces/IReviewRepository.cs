using AISA.Domain.Entities;

namespace AISA.Domain.Interfaces;

/// <summary>
/// Repository pentru gestionarea recenziilor.
/// </summary>
public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Review>> GetByBusinessProfileIdAsync(Guid businessProfileId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Review>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Review> AddAsync(Review review, CancellationToken cancellationToken = default);
    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetReviewCountForMonthAsync(Guid businessProfileId, DateTime month, CancellationToken cancellationToken = default);
}

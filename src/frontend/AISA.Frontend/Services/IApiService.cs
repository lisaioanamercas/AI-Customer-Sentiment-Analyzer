using AISA.Frontend.Models;

namespace AISA.Frontend.Services;

/// <summary>
/// Contract pentru serviciul de comunicare cu API-ul backend.
/// </summary>
public interface IApiService
{
    // Reviews
    Task<List<ReviewModel>> GetReviewsAsync(Guid businessProfileId);
    Task<ReviewModel> AddReviewAsync(string content, string? authorName, string source, Guid businessProfileId);
    Task DeleteReviewAsync(Guid reviewId);
    Task<List<SentimentTrendModel>> GetSentimentTrendsAsync(Guid businessProfileId, DateTime? fromDate = null, DateTime? toDate = null);

    // Business Profiles
    Task<BusinessProfileModel?> GetBusinessProfileAsync(Guid id);
    Task<BusinessProfileModel> CreateBusinessProfileAsync(string name, string? description, string? category, string? address);
}

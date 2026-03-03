namespace AISA.Domain.Interfaces;

/// <summary>
/// Contract pentru clientul de scraping recenzii din serviciul Python AI.
/// </summary>
public interface IReviewScraperClient
{
    /// <summary>
    /// Apelează serviciul Python /api/scrape și returnează recenziile brute.
    /// </summary>
    /// <param name="url">URL-ul paginii sursă (Google Maps sau TripAdvisor)</param>
    /// <param name="source">Sursa: "Google" sau "TripAdvisor"</param>
    /// <param name="sortBy">Sortare: "newest" sau "relevant"</param>
    /// <param name="maxCount">Numărul maxim de recenzii</param>
    Task<IReadOnlyList<ScrapedReviewData>> ScrapeReviewsAsync(
        string url,
        string source,
        string sortBy,
        int maxCount,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Date brute ale unei recenzii preluate din scraping.
/// </summary>
public class ScrapedReviewData
{
    public string ExternalId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? AuthorName { get; set; }
    public DateTime? ReviewedAt { get; set; }
}

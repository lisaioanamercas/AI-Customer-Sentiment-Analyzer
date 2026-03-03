using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AISA.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AISA.Infrastructure.ExternalServices;

/// <summary>
/// Client HTTP care apelează Python AI microservice-ul pentru scraping recenzii.
/// POST /api/scrape → returnează lista de recenzii brute.
/// </summary>
public class ReviewScraperClient : IReviewScraperClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReviewScraperClient> _logger;

    public ReviewScraperClient(HttpClient httpClient, ILogger<ReviewScraperClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ScrapedReviewData>> ScrapeReviewsAsync(
        string url,
        string source,
        string sortBy,
        int maxCount,
        CancellationToken cancellationToken = default)
    {
        var request = new ScrapeRequest
        {
            Url = url,
            Source = source,
            SortBy = sortBy,
            MaxCount = maxCount
        };

        _logger.LogInformation("Scraping {MaxCount} recenzii din {Source} ({Url})...", maxCount, source, url);

        var response = await _httpClient.PostAsJsonAsync("/api/scrape", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<ScrapeResponseItem>>(
            cancellationToken: cancellationToken) ?? new List<ScrapeResponseItem>();

        _logger.LogInformation("Scraping completat: {Count} recenzii primite.", result.Count);

        return result.Select(item => new ScrapedReviewData
        {
            ExternalId = item.ExternalId,
            Content = item.Content,
            AuthorName = item.AuthorName,
            ReviewedAt = item.ReviewedAt.HasValue
                ? DateTime.SpecifyKind(item.ReviewedAt.Value, DateTimeKind.Utc)
                : null
        }).ToList();
    }

    // ── Request / Response DTOs (interne) ────────────────────────────────────

    private class ScrapeRequest
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("sort_by")]
        public string SortBy { get; set; } = "newest";

        [JsonPropertyName("max_count")]
        public int MaxCount { get; set; } = 20;
    }

    private class ScrapeResponseItem
    {
        [JsonPropertyName("external_id")]
        public string ExternalId { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("author_name")]
        public string? AuthorName { get; set; }

        [JsonPropertyName("reviewed_at")]
        public DateTime? ReviewedAt { get; set; }
    }
}

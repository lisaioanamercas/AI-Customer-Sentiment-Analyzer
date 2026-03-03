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

        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/scrape", request, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Scraping service returned error {StatusCode}: {ErrorContent}", 
                    response.StatusCode, errorContent);
                
                throw new InvalidOperationException(
                    $"Python scraping service failed with status {response.StatusCode}. " +
                    $"Details: {errorContent}. " +
                    $"Make sure the AI service is running and Playwright browsers are installed.");
            }

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
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Cannot connect to scraping service at {BaseAddress}", _httpClient.BaseAddress);
            throw new InvalidOperationException(
                $"Unable to connect to Python AI service at {_httpClient.BaseAddress}. " +
                "Ensure the service is running on http://localhost:8000", ex);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during scraping");
            throw new InvalidOperationException($"Scraping failed: {ex.Message}", ex);
        }
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
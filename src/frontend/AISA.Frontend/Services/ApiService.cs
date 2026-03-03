using System.Net.Http.Json;
using AISA.Frontend.Models;

namespace AISA.Frontend.Services;

/// <summary>
/// Implementarea serviciului de comunicare cu API-ul backend.
/// Wrapper peste HttpClient cu metode tipizate.
/// </summary>
public class ApiService : IApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http)
    {
        _http = http;
    }

    // ===== Reviews =====

    public async Task<List<ReviewModel>> GetReviewsAsync(Guid businessProfileId)
    {
        var result = await _http.GetFromJsonAsync<List<ReviewModel>>($"api/reviews/{businessProfileId}");
        return result ?? new List<ReviewModel>();
    }

    public async Task<ReviewModel> AddReviewAsync(string content, string? authorName, string source, Guid businessProfileId)
    {
        var request = new
        {
            content,
            authorName,
            source,
            businessProfileId
        };

        var response = await _http.PostAsJsonAsync("api/reviews", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReviewModel>();
        return result ?? throw new InvalidOperationException("Răspuns invalid de la server.");
    }

    public async Task DeleteReviewAsync(Guid reviewId)
    {
        var response = await _http.DeleteAsync($"api/reviews/{reviewId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<SentimentTrendModel>> GetSentimentTrendsAsync(
        Guid businessProfileId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var url = $"api/reviews/{businessProfileId}/trends";
        var queryParams = new List<string>();

        if (fromDate.HasValue)
            queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
        if (toDate.HasValue)
            queryParams.Add($"toDate={toDate.Value:yyyy-MM-dd}");

        if (queryParams.Any())
            url += "?" + string.Join("&", queryParams);

        var result = await _http.GetFromJsonAsync<List<SentimentTrendModel>>(url);
        return result ?? new List<SentimentTrendModel>();
    }

    // ===== Business Profiles =====

    public async Task<BusinessProfileModel?> GetBusinessProfileAsync(Guid id)
    {
        try
        {
            return await _http.GetFromJsonAsync<BusinessProfileModel>($"api/businessprofiles/{id}");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<BusinessProfileModel?> GetMyBusinessProfileAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<BusinessProfileModel>("api/businessprofiles/my");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<BusinessProfileModel> CreateBusinessProfileAsync(
        string name, string? description, string? category, string? address,
        string? googleMapsUrl, string? tripAdvisorUrl)
    {
        var request = new { name, description, category, address, googleMapsUrl, tripAdvisorUrl };
        var response = await _http.PostAsJsonAsync("api/businessprofiles", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<BusinessProfileModel>();
        return result ?? throw new InvalidOperationException("Răspuns invalid de la server.");
    }

    public async Task<BusinessProfileModel> UpdateBusinessProfileAsync(
        Guid id, string name, string? description, string? category, string? address,
        string? googleMapsUrl, string? tripAdvisorUrl)
    {
        var request = new { name, description, category, address, googleMapsUrl, tripAdvisorUrl };
        var response = await _http.PutAsJsonAsync($"api/businessprofiles/{id}", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<BusinessProfileModel>();
        return result ?? throw new InvalidOperationException("Răspuns invalid de la server.");
    }

    public async Task<ScrapeResultModel> ScrapeAndImportAsync(
        Guid businessProfileId, string source, string sortBy, int maxCount)
    {
        var request = new { businessProfileId, source, sortBy, maxCount };
        var response = await _http.PostAsJsonAsync("api/reviews/scrape", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ScrapeResultModel>();
        return result ?? throw new InvalidOperationException("Răspuns invalid de la server.");
    }
}

using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AISA.Domain.Entities;
using AISA.Domain.Enums;
using AISA.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AISA.Infrastructure.ExternalServices;

/// <summary>
/// Client HTTP pentru comunicarea cu microserviciul AI Python.
/// Trimite textul recenziei și primește eticheta + scorul de sentiment.
/// </summary>
public class AiSentimentClient : ISentimentAnalysisService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AiSentimentClient> _logger;

    public AiSentimentClient(HttpClient httpClient, ILogger<AiSentimentClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<SentimentResult> AnalyzeAsync(string text, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var request = new AiSentimentRequest { Text = text };
            var response = await _httpClient.PostAsJsonAsync("/api/analyze", request, cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AiSentimentResponse>(cancellationToken: cancellationToken);

            stopwatch.Stop();

            if (result is null)
            {
                throw new InvalidOperationException("Răspuns gol de la serviciul AI.");
            }

            _logger.LogInformation("Analiză sentiment completată în {ElapsedMs}ms: {Label} ({Score:F3})",
                stopwatch.ElapsedMilliseconds, result.Label, result.Score);

            return new SentimentResult
            {
                Label = ParseLabel(result.Label),
                Score = result.Score,
                ProcessingTimeMs = (int)stopwatch.ElapsedMilliseconds
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Eroare la comunicarea cu serviciul AI");
            throw new InvalidOperationException("Serviciul de analiză AI nu este disponibil.", ex);
        }
    }

    private static SentimentLabel ParseLabel(string label)
    {
        return label.ToLowerInvariant() switch
        {
            "positive" or "pozitiv" => SentimentLabel.Positive,
            "negative" or "negativ" => SentimentLabel.Negative,
            _ => SentimentLabel.Neutral
        };
    }

    // DTO-uri interne pentru comunicarea cu API-ul Python
    private class AiSentimentRequest
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    private class AiSentimentResponse
    {
        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;

        [JsonPropertyName("score")]
        public double Score { get; set; }
    }
}

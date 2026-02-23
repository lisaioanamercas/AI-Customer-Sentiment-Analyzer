using AISA.Domain.Entities;

namespace AISA.Domain.Interfaces;

/// <summary>
/// Contract pentru serviciul de analiză a sentimentului.
/// Implementarea va fi în Infrastructure Layer (HTTP client → Python AI).
/// </summary>
public interface ISentimentAnalysisService
{
    /// <summary>
    /// Analizează sentimentul textului furnizat.
    /// </summary>
    /// <param name="text">Textul recenziei de analizat</param>
    /// <param name="cancellationToken">Token de anulare</param>
    /// <returns>Rezultatul analizei de sentiment</returns>
    Task<SentimentResult> AnalyzeAsync(string text, CancellationToken cancellationToken = default);
}

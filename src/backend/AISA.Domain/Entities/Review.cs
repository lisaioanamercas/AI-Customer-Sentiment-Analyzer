using AISA.Domain.Common;

namespace AISA.Domain.Entities;

/// <summary>
/// O recenzie primită de la un client al afacerii.
/// Conține textul original și rezultatul analizei de sentiment.
/// </summary>
public class Review : BaseEntity
{
    /// <summary>Textul complet al recenziei</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Numele autorului recenziei (opțional)</summary>
    public string? AuthorName { get; set; }

    /// <summary>Sursa recenziei (ex: "Google", "TripAdvisor", "Manual")</summary>
    public string Source { get; set; } = "Manual";

    /// <summary>ID extern din platforma sursă — folosit pentru deduplicare la scraping</summary>
    public string? ExternalId { get; set; }

    /// <summary>Data originală a recenziei de pe platformă (distinct de CreatedAt)</summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>FK către profilul de business</summary>
    public Guid BusinessProfileId { get; set; }

    /// <summary>Navigare către profilul de business</summary>
    public BusinessProfile BusinessProfile { get; set; } = null!;

    /// <summary>Rezultatul analizei de sentiment (1:1)</summary>
    public SentimentResult? SentimentResult { get; set; }
}

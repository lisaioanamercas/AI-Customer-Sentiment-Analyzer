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

    /// <summary>Sursa recenziei (ex: "Google", "Facebook", "Manual")</summary>
    public string Source { get; set; } = "Manual";

    /// <summary>FK către profilul de business</summary>
    public Guid BusinessProfileId { get; set; }

    /// <summary>Navigare către profilul de business</summary>
    public BusinessProfile BusinessProfile { get; set; } = null!;

    /// <summary>Rezultatul analizei de sentiment (1:1)</summary>
    public SentimentResult? SentimentResult { get; set; }
}

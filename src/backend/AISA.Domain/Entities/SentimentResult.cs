using AISA.Domain.Common;
using AISA.Domain.Enums;

namespace AISA.Domain.Entities;

/// <summary>
/// Rezultatul analizei de sentiment returnate de modulul AI.
/// Legat 1:1 de o recenzie.
/// </summary>
public class SentimentResult : BaseEntity
{
    /// <summary>Eticheta sentimentului: Pozitiv, Negativ, Neutru</summary>
    public SentimentLabel Label { get; set; }

    /// <summary>Scorul de încredere al modelului (0.0 - 1.0)</summary>
    public double Score { get; set; }

    /// <summary>Versiunea modelului AI utilizat</summary>
    public string ModelVersion { get; set; } = "distilbert-base-uncased";

    /// <summary>Timpul de procesare în milisecunde</summary>
    public int ProcessingTimeMs { get; set; }

    /// <summary>FK către recenzie</summary>
    public Guid ReviewId { get; set; }

    /// <summary>Navigare către recenzie</summary>
    public Review Review { get; set; } = null!;
}

using AISA.Domain.Enums;

namespace AISA.Application.Reviews.DTOs;

/// <summary>
/// DTO pentru trend-uri de sentiment — folosit pentru grafice dashboard.
/// </summary>
public class SentimentTrendDto
{
    /// <summary>Data (grupată pe zi/săptămână/lună)</summary>
    public DateTime Date { get; set; }

    /// <summary>Număr total de recenzii în perioada respectivă</summary>
    public int TotalReviews { get; set; }

    /// <summary>Număr recenzii pozitive</summary>
    public int PositiveCount { get; set; }

    /// <summary>Număr recenzii negative</summary>
    public int NegativeCount { get; set; }

    /// <summary>Număr recenzii neutre</summary>
    public int NeutralCount { get; set; }

    /// <summary>Scorul mediu de sentiment (0.0 - 1.0)</summary>
    public double AverageScore { get; set; }
}

namespace AISA.Frontend.Models;

/// <summary>
/// Model pentru trend-uri de sentiment (mirror al SentimentTrendDto).
/// </summary>
public class SentimentTrendModel
{
    public DateTime Date { get; set; }
    public int TotalReviews { get; set; }
    public int PositiveCount { get; set; }
    public int NegativeCount { get; set; }
    public int NeutralCount { get; set; }
    public double AverageScore { get; set; }
}

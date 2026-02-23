namespace AISA.Frontend.Models;

/// <summary>
/// Modelul pentru o recenzie (mirror al ReviewDto din backend).
/// </summary>
public class ReviewModel
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? AuthorName { get; set; }
    public string Source { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? SentimentLabel { get; set; }
    public double? SentimentScore { get; set; }
    public Guid BusinessProfileId { get; set; }
}

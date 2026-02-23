using AISA.Domain.Enums;

namespace AISA.Application.Reviews.DTOs;

/// <summary>
/// DTO pentru o recenzie — folosit în răspunsurile API.
/// </summary>
public class ReviewDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? AuthorName { get; set; }
    public string Source { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Sentiment
    public SentimentLabel? SentimentLabel { get; set; }
    public double? SentimentScore { get; set; }

    public Guid BusinessProfileId { get; set; }
}

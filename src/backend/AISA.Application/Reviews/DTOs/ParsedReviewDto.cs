namespace AISA.Application.Reviews.DTOs;

public record ParsedReviewDto(
    string Content,
    string? AuthorName,
    string? Source,
    string? DateStr,
    int? Rating
);

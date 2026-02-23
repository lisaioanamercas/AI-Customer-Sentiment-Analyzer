using AISA.Application.Reviews.DTOs;
using MediatR;

namespace AISA.Application.Reviews.Commands.AddReview;

/// <summary>
/// Command pentru adăugarea unei recenzii noi.
/// Declanșează analiza de sentiment prin AI și salvarea în DB.
/// </summary>
public record AddReviewCommand : IRequest<ReviewDto>
{
    /// <summary>Textul recenziei</summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>Numele autorului (opțional)</summary>
    public string? AuthorName { get; init; }

    /// <summary>Sursa recenziei (Google, Facebook, Manual)</summary>
    public string Source { get; init; } = "Manual";

    /// <summary>ID-ul profilului de business</summary>
    public Guid BusinessProfileId { get; init; }
}

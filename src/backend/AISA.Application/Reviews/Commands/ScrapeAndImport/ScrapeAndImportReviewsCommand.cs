using AISA.Application.Reviews.DTOs;
using MediatR;

namespace AISA.Application.Reviews.Commands.ScrapeAndImport;

/// <summary>
/// Command pentru scraping recenzii din Google sau TripAdvisor și importul lor cu analiză AI.
/// </summary>
public record ScrapeAndImportReviewsCommand : IRequest<ScrapeResultDto>
{
    /// <summary>ID-ul profilului de business pentru care se importă recenzii.</summary>
    public Guid BusinessProfileId { get; init; }

    /// <summary>Sursa: "Google" sau "TripAdvisor"</summary>
    public string Source { get; init; } = "Google";

    /// <summary>Sortare: "newest" sau "relevant"</summary>
    public string SortBy { get; init; } = "newest";

    /// <summary>Numărul maxim de recenzii de preluat (max 50).</summary>
    public int MaxCount { get; init; } = 20;
}

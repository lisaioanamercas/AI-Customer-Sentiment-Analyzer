using Fluxor;
using AISA.Frontend.Models;

namespace AISA.Frontend.Store.Reviews;

/// <summary>
/// Single source of truth for all review-related data in the app.
/// </summary>
[FeatureState]
public record ReviewState
{
    /// <summary>True while fetching the reviews list from the API.</summary>
    public bool IsLoading { get; init; } = false;

    /// <summary>True while the AI analysis job is running.</summary>
    public bool IsAnalyzing { get; init; } = false;

    /// <summary>The current list of reviews for the active business profile.</summary>
    public IReadOnlyList<ReviewModel> Reviews { get; init; } = [];

    /// <summary>Sentiment trend data for the dashboard.</summary>
    public IReadOnlyList<SentimentTrendModel> Trends { get; init; } = [];

    /// <summary>Error message from the last failed fetch or analysis.</summary>
    public string? ErrorMessage { get; init; }

    /// <summary>Timestamp of the last completed analysis run.</summary>
    public DateTime? LastAnalyzedAt { get; init; }
}

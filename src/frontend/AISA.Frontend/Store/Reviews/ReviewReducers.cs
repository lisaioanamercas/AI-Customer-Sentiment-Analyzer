using Fluxor;

namespace AISA.Frontend.Store.Reviews;

/// <summary>
/// Pure reducer functions for ReviewState.
/// Each action gets a dedicated method that returns a new state record.
/// </summary>
public static class ReviewReducers
{
    // ── Fetch ─────────────────────────────────────────────────────────────

    [ReducerMethod(typeof(FetchReviewsAction))]
    public static ReviewState OnFetchStarted(ReviewState state) =>
        state with { IsLoading = true, ErrorMessage = null };

    [ReducerMethod]
    public static ReviewState OnReviewsLoaded(ReviewState state, ReviewsLoadedAction action) =>
        state with { IsLoading = false, Reviews = action.Reviews, ErrorMessage = null };

    [ReducerMethod]
    public static ReviewState OnReviewsLoadFailed(ReviewState state, ReviewsLoadFailedAction action) =>
        state with { IsLoading = false, ErrorMessage = action.ErrorMessage };

    // ── Trends ────────────────────────────────────────────────────────────

    [ReducerMethod(typeof(FetchTrendsAction))]
    public static ReviewState OnFetchTrendsStarted(ReviewState state) =>
        state with { IsLoading = true, ErrorMessage = null };

    [ReducerMethod]
    public static ReviewState OnTrendsLoaded(ReviewState state, TrendsLoadedAction action) =>
        state with { IsLoading = false, Trends = action.Trends, ErrorMessage = null };

    [ReducerMethod]
    public static ReviewState OnTrendsLoadFailed(ReviewState state, TrendsLoadFailedAction action) =>
        state with { IsLoading = false, ErrorMessage = action.ErrorMessage };

    // ── Add ───────────────────────────────────────────────────────────────

    [ReducerMethod]
    public static ReviewState OnReviewAdded(ReviewState state, ReviewAddedAction action) =>
        state with { Reviews = [action.Review, ..state.Reviews] };

    // ── Delete ────────────────────────────────────────────────────────────

    [ReducerMethod]
    public static ReviewState OnReviewDeleted(ReviewState state, ReviewDeletedAction action) =>
        state with { Reviews = state.Reviews.Where(r => r.Id != action.ReviewId).ToList() };

    // ── AI Analysis ───────────────────────────────────────────────────────

    [ReducerMethod(typeof(AnalyzeReviewsAction))]
    public static ReviewState OnAnalysisStarted(ReviewState state) =>
        state with { IsAnalyzing = true, ErrorMessage = null };

    [ReducerMethod]
    public static ReviewState OnReviewsAnalyzed(ReviewState state, ReviewsAnalyzedAction action) =>
        state with { IsAnalyzing = false, LastAnalyzedAt = DateTime.UtcNow };
}

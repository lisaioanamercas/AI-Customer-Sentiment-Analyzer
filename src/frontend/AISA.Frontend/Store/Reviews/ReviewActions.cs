using AISA.Frontend.Models;

namespace AISA.Frontend.Store.Reviews;

// ── Fetch ─────────────────────────────────────────────────────────────────────

/// <summary>Dispatch to load all reviews for the given business profile.</summary>
public record FetchReviewsAction(Guid BusinessProfileId);

/// <summary>Dispatched when the reviews fetch succeeds.</summary>
public record ReviewsLoadedAction(IReadOnlyList<ReviewModel> Reviews);

/// <summary>Dispatched when the reviews fetch fails.</summary>
public record ReviewsLoadFailedAction(string ErrorMessage);

// ── Add ───────────────────────────────────────────────────────────────────────

/// <summary>Dispatch to manually add a single review.</summary>
public record AddReviewAction(string Content, string? AuthorName, string Source, Guid BusinessProfileId);

/// <summary>Dispatched when the review was successfully created on the backend.</summary>
public record ReviewAddedAction(ReviewModel Review);

// ── Delete ────────────────────────────────────────────────────────────────────

/// <summary>Dispatch to delete a review by ID.</summary>
public record DeleteReviewAction(Guid ReviewId);

/// <summary>Dispatched when the review was successfully deleted.</summary>
public record ReviewDeletedAction(Guid ReviewId);

// ── AI Analysis ───────────────────────────────────────────────────────────────

/// <summary>Dispatch to start an AI sentiment analysis run.</summary>
public record AnalyzeReviewsAction(Guid BusinessProfileId, int MaxCount);

/// <summary>Dispatched when the analysis run completes successfully.</summary>
public record ReviewsAnalyzedAction(int Analyzed, int Failed);

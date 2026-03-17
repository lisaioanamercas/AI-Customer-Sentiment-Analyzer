using Fluxor;
using AISA.Frontend.Services;
using AISA.Frontend.Store.Authentication;

namespace AISA.Frontend.Store.Reviews;

public class ReviewEffects
{
    private readonly IApiService _apiService;

    public ReviewEffects(IApiService apiService)
    {
        _apiService = apiService;
    }

    [EffectMethod]
    public Task HandleAuthEvents(AuthInitializedAction action, IDispatcher dispatcher)
    {
        if (action.BusinessProfileId.HasValue)
        {
            dispatcher.Dispatch(new FetchReviewsAction(action.BusinessProfileId.Value));
            dispatcher.Dispatch(new FetchTrendsAction(action.BusinessProfileId.Value));
        }
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandleLoginSuccess(LoginSuccessAction action, IDispatcher dispatcher)
    {
        if (action.BusinessProfileId.HasValue)
        {
            dispatcher.Dispatch(new FetchReviewsAction(action.BusinessProfileId.Value));
            dispatcher.Dispatch(new FetchTrendsAction(action.BusinessProfileId.Value));
        }
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandleRegisterSuccess(RegisterSuccessAction action, IDispatcher dispatcher)
    {
        if (action.BusinessProfileId.HasValue)
        {
            dispatcher.Dispatch(new FetchReviewsAction(action.BusinessProfileId.Value));
            dispatcher.Dispatch(new FetchTrendsAction(action.BusinessProfileId.Value));
        }
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleFetchReviews(FetchReviewsAction action, IDispatcher dispatcher)
    {
        try
        {
            var reviews = await _apiService.GetReviewsAsync(action.BusinessProfileId);
            dispatcher.Dispatch(new ReviewsLoadedAction(reviews));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new ReviewsLoadFailedAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleFetchTrends(FetchTrendsAction action, IDispatcher dispatcher)
    {
        try
        {
            var trends = await _apiService.GetSentimentTrendsAsync(action.BusinessProfileId);
            dispatcher.Dispatch(new TrendsLoadedAction(trends));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new TrendsLoadFailedAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleAddReview(AddReviewAction action, IDispatcher dispatcher)
    {
        try
        {
            var review = await _apiService.AddReviewAsync(action.Content, action.AuthorName, action.Source, action.BusinessProfileId);
            dispatcher.Dispatch(new ReviewAddedAction(review));
        }
        catch (Exception)
        {
            // Error handling could be improved with a dedicated action
        }
    }

    [EffectMethod]
    public async Task HandleDeleteReview(DeleteReviewAction action, IDispatcher dispatcher)
    {
        try
        {
            await _apiService.DeleteReviewAsync(action.ReviewId);
            dispatcher.Dispatch(new ReviewDeletedAction(action.ReviewId));
        }
        catch (Exception)
        {
            // Error handling
        }
    }

    [EffectMethod]
    public async Task HandleAnalyzeReviews(AnalyzeReviewsAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await _apiService.AnalyzeReviewsAsync(action.BusinessProfileId, action.MaxCount);
            dispatcher.Dispatch(new ReviewsAnalyzedAction(result.Analyzed, result.Failed));
            
            // Refresh reviews and trends after analysis
            dispatcher.Dispatch(new FetchReviewsAction(action.BusinessProfileId));
            dispatcher.Dispatch(new FetchTrendsAction(action.BusinessProfileId));
        }
        catch (Exception)
        {
            // Error handling
        }
    }
}

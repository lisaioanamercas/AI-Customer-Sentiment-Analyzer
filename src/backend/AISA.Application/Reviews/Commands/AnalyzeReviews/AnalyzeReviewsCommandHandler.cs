using AISA.Domain.Interfaces;
using MediatR;

namespace AISA.Application.Reviews.Commands.AnalyzeReviews;

public class AnalyzeReviewsCommandHandler : IRequestHandler<AnalyzeReviewsCommand, AnalyzeReviewsResult>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ISentimentAnalysisService _sentimentService;

    public AnalyzeReviewsCommandHandler(
        IReviewRepository reviewRepository,
        ISentimentAnalysisService sentimentService)
    {
        _reviewRepository = reviewRepository;
        _sentimentService = sentimentService;
    }

    public async Task<AnalyzeReviewsResult> Handle(AnalyzeReviewsCommand request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetUnanalyzedAsync(
            request.BusinessProfileId, request.MaxCount, cancellationToken);

        int analyzed = 0;
        int failed = 0;

        foreach (var review in reviews)
        {
            try
            {
                var sentiment = await _sentimentService.AnalyzeAsync(review.Content, cancellationToken);
                sentiment.ReviewId = review.Id;
                review.SentimentResult = sentiment;

                await _reviewRepository.UpdateAsync(review, cancellationToken);
                analyzed++;
            }
            catch
            {
                failed++;
            }
        }

        return new AnalyzeReviewsResult(analyzed, failed);
    }
}

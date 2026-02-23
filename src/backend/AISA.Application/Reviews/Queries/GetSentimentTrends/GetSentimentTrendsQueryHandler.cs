using AISA.Application.Reviews.DTOs;
using AISA.Domain.Enums;
using AISA.Domain.Interfaces;
using MediatR;

namespace AISA.Application.Reviews.Queries.GetSentimentTrends;

public class GetSentimentTrendsQueryHandler : IRequestHandler<GetSentimentTrendsQuery, IReadOnlyList<SentimentTrendDto>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetSentimentTrendsQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IReadOnlyList<SentimentTrendDto>> Handle(GetSentimentTrendsQuery request, CancellationToken cancellationToken)
    {
        var fromDate = request.FromDate ?? DateTime.UtcNow.AddDays(-30);
        var toDate = request.ToDate ?? DateTime.UtcNow;

        var reviews = await _reviewRepository.GetByBusinessProfileIdAsync(request.BusinessProfileId, cancellationToken);

        // Filtrăm după perioada cerută și grupăm pe zi
        var trends = reviews
            .Where(r => r.CreatedAt >= fromDate && r.CreatedAt <= toDate && r.SentimentResult != null)
            .GroupBy(r => r.CreatedAt.Date)
            .OrderBy(g => g.Key)
            .Select(g => new SentimentTrendDto
            {
                Date = g.Key,
                TotalReviews = g.Count(),
                PositiveCount = g.Count(r => r.SentimentResult!.Label == SentimentLabel.Positive),
                NegativeCount = g.Count(r => r.SentimentResult!.Label == SentimentLabel.Negative),
                NeutralCount = g.Count(r => r.SentimentResult!.Label == SentimentLabel.Neutral),
                AverageScore = g.Average(r => r.SentimentResult!.Score)
            })
            .ToList();

        return trends;
    }
}

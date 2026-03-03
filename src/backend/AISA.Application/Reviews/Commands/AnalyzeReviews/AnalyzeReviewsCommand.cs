using MediatR;

namespace AISA.Application.Reviews.Commands.AnalyzeReviews;

public record AnalyzeReviewsCommand(
    Guid BusinessProfileId,
    int MaxCount
) : IRequest<AnalyzeReviewsResult>;

public record AnalyzeReviewsResult(int Analyzed, int Failed);

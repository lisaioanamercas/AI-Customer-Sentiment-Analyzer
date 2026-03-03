using AISA.Application.Reviews.Commands.ImportReviews;
using AISA.Domain.Entities;
using AISA.Domain.Interfaces;
using MediatR;
using System.Globalization;

namespace AISA.Application.Reviews.Commands.ImportReviews;

public class ImportReviewsCommandHandler : IRequestHandler<ImportReviewsCommand, int>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IBusinessProfileRepository _businessProfileRepository;

    public ImportReviewsCommandHandler(
        IReviewRepository reviewRepository,
        IBusinessProfileRepository businessProfileRepository)
    {
        _reviewRepository = reviewRepository;
        _businessProfileRepository = businessProfileRepository;
    }

    public async Task<int> Handle(ImportReviewsCommand request, CancellationToken cancellationToken)
    {
        var businessProfile = await _businessProfileRepository.GetByIdAsync(request.BusinessProfileId, cancellationToken);
        if (businessProfile == null)
            throw new Exception("BusinessProfile not found");

        var newReviews = new List<Review>();

        foreach (var parsedDto in request.Reviews)
        {
            if (string.IsNullOrWhiteSpace(parsedDto.Content))
                continue;

            DateTime? reviewedAt = null;
            if (!string.IsNullOrWhiteSpace(parsedDto.DateStr) && 
                DateTime.TryParse(parsedDto.DateStr, CultureInfo.InvariantCulture, out var parsedDate))
            {
                reviewedAt = parsedDate;
            }

            var review = new Review
            {
                Content = parsedDto.Content,
                AuthorName = parsedDto.AuthorName,
                Source = string.IsNullOrWhiteSpace(parsedDto.Source) ? "Manual" : parsedDto.Source,
                ReviewedAt = reviewedAt,
                Rating = parsedDto.Rating,
                BusinessProfileId = request.BusinessProfileId
            };

            newReviews.Add(review);
        }

        foreach (var review in newReviews)
        {
            await _reviewRepository.AddAsync(review, cancellationToken);
        }

        return newReviews.Count;
    }
}

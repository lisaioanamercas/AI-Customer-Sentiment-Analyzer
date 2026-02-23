using AISA.Application.Reviews.DTOs;
using AISA.Domain.Entities;
using AISA.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AISA.Application.Reviews.Commands.AddReview;

/// <summary>
/// Handler pentru AddReviewCommand.
/// Orchestrăm: salvare recenzie → analiză AI → atașare rezultat → returnare DTO.
/// </summary>
public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ISentimentAnalysisService _sentimentService;
    private readonly IMapper _mapper;

    public AddReviewCommandHandler(
        IReviewRepository reviewRepository,
        ISentimentAnalysisService sentimentService,
        IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _sentimentService = sentimentService;
        _mapper = mapper;
    }

    public async Task<ReviewDto> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        // 1. Creăm entitatea Review
        var review = new Review
        {
            Content = request.Content,
            AuthorName = request.AuthorName,
            Source = request.Source,
            BusinessProfileId = request.BusinessProfileId
        };

        // 2. Analizăm sentimentul prin microserviciul AI
        var sentimentResult = await _sentimentService.AnalyzeAsync(request.Content, cancellationToken);
        sentimentResult.ReviewId = review.Id;
        review.SentimentResult = sentimentResult;

        // 3. Salvăm în baza de date
        var savedReview = await _reviewRepository.AddAsync(review, cancellationToken);

        // 4. Mapăm și returnăm DTO-ul
        return _mapper.Map<ReviewDto>(savedReview);
    }
}

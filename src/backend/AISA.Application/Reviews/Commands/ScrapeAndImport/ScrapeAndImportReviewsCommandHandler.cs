using AISA.Application.Reviews.DTOs;
using AISA.Domain.Entities;
using AISA.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AISA.Application.Reviews.Commands.ScrapeAndImport;

/// <summary>
/// Handler pentru ScrapeAndImportReviewsCommand.
/// 1. Obține URL-ul din profilul de business.
/// 2. Apelează clientul de scraping (Python AI service).
/// 3. Deduplicăm pe baza ExternalId.
/// 4. Analizăm sentimentul fiecărei recenzii noi și salvăm.
/// </summary>
public class ScrapeAndImportReviewsCommandHandler
    : IRequestHandler<ScrapeAndImportReviewsCommand, ScrapeResultDto>
{
    private readonly IBusinessProfileRepository _profileRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IReviewScraperClient _scraperClient;
    private readonly ISentimentAnalysisService _sentimentService;

    public ScrapeAndImportReviewsCommandHandler(
        IBusinessProfileRepository profileRepository,
        IReviewRepository reviewRepository,
        IReviewScraperClient scraperClient,
        ISentimentAnalysisService sentimentService)
    {
        _profileRepository = profileRepository;
        _reviewRepository = reviewRepository;
        _scraperClient = scraperClient;
        _sentimentService = sentimentService;
    }

    public async Task<ScrapeResultDto> Handle(
        ScrapeAndImportReviewsCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Obținem profilul și URL-ul sursei
        var profile = await _profileRepository.GetByIdAsync(request.BusinessProfileId, cancellationToken)
            ?? throw new KeyNotFoundException($"Business profile {request.BusinessProfileId} not found.");

        var url = request.Source.ToLowerInvariant() switch
        {
            "tripadvisor" => profile.TripAdvisorUrl,
            _ => profile.GoogleMapsUrl
        };

        if (string.IsNullOrWhiteSpace(url))
            throw new InvalidOperationException(
                $"URL-ul pentru sursa '{request.Source}' nu este configurat în profilul afacerii. " +
                "Adaugă URL-ul în pagina Setări.");

        // 2. Scraping prin Python AI service
        var scraped = await _scraperClient.ScrapeReviewsAsync(
            url,
            request.Source,
            request.SortBy,
            Math.Clamp(request.MaxCount, 1, 50),
            cancellationToken);

        // 3. Obținem ID-urile externe existente pentru a deduplica
        var existingReviews = await _reviewRepository.GetByBusinessProfileIdAsync(
            request.BusinessProfileId, cancellationToken);
        var existingExternalIds = existingReviews
            .Where(r => r.ExternalId is not null)
            .Select(r => r.ExternalId!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        int newCount = 0;
        int skippedCount = 0;

        // 4. Procesăm recenziile noi
        foreach (var scraped_review in scraped)
        {
            if (existingExternalIds.Contains(scraped_review.ExternalId))
            {
                skippedCount++;
                continue;
            }

            var review = new Review
            {
                Content = scraped_review.Content,
                AuthorName = scraped_review.AuthorName,
                Source = request.Source,
                ExternalId = scraped_review.ExternalId,
                ReviewedAt = scraped_review.ReviewedAt,
                BusinessProfileId = request.BusinessProfileId
            };

            // Analiză sentiment
            var sentimentResult = await _sentimentService.AnalyzeAsync(review.Content, cancellationToken);
            sentimentResult.ReviewId = review.Id;
            review.SentimentResult = sentimentResult;

            await _reviewRepository.AddAsync(review, cancellationToken);
            newCount++;
        }

        return new ScrapeResultDto { NewCount = newCount, SkippedCount = skippedCount };
    }
}

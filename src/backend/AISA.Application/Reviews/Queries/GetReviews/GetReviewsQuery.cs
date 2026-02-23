using AISA.Application.Reviews.DTOs;
using MediatR;

namespace AISA.Application.Reviews.Queries.GetReviews;

/// <summary>
/// Query pentru obținerea recenziilor unui business profile.
/// </summary>
public record GetReviewsQuery(Guid BusinessProfileId) : IRequest<IReadOnlyList<ReviewDto>>;

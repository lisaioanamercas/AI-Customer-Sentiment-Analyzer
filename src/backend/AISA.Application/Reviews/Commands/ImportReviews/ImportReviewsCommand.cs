using AISA.Application.Reviews.DTOs;
using MediatR;

namespace AISA.Application.Reviews.Commands.ImportReviews;

public record ImportReviewsCommand(
    Guid BusinessProfileId,
    List<ParsedReviewDto> Reviews
) : IRequest<int>; // Returns the number of imported reviews

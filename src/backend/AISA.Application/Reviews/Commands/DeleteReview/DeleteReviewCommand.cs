using MediatR;

namespace AISA.Application.Reviews.Commands.DeleteReview;

/// <summary>
/// Command pentru ștergerea unei recenzii.
/// </summary>
public record DeleteReviewCommand(Guid Id) : IRequest<Unit>;

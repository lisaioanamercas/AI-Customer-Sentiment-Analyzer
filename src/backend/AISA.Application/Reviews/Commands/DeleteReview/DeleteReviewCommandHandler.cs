using AISA.Domain.Interfaces;
using MediatR;

namespace AISA.Application.Reviews.Commands.DeleteReview;

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Unit>
{
    private readonly IReviewRepository _reviewRepository;

    public DeleteReviewCommandHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        await _reviewRepository.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}

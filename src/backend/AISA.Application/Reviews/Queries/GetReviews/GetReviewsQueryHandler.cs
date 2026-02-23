using AISA.Application.Reviews.DTOs;
using AISA.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AISA.Application.Reviews.Queries.GetReviews;

public class GetReviewsQueryHandler : IRequestHandler<GetReviewsQuery, IReadOnlyList<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public GetReviewsQueryHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ReviewDto>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetByBusinessProfileIdAsync(request.BusinessProfileId, cancellationToken);
        return _mapper.Map<IReadOnlyList<ReviewDto>>(reviews);
    }
}

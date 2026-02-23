using AISA.Application.BusinessProfiles.DTOs;
using AISA.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AISA.Application.BusinessProfiles.Queries.GetBusinessProfile;

public class GetBusinessProfileQueryHandler : IRequestHandler<GetBusinessProfileQuery, BusinessProfileDto?>
{
    private readonly IBusinessProfileRepository _repository;
    private readonly IMapper _mapper;

    public GetBusinessProfileQueryHandler(IBusinessProfileRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<BusinessProfileDto?> Handle(GetBusinessProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return profile is null ? null : _mapper.Map<BusinessProfileDto>(profile);
    }
}

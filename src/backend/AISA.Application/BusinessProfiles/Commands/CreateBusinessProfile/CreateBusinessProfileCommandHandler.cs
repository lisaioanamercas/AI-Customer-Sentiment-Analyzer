using AISA.Application.BusinessProfiles.DTOs;
using AISA.Domain.Entities;
using AISA.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AISA.Application.BusinessProfiles.Commands.CreateBusinessProfile;

public class CreateBusinessProfileCommandHandler : IRequestHandler<CreateBusinessProfileCommand, BusinessProfileDto>
{
    private readonly IBusinessProfileRepository _repository;
    private readonly IMapper _mapper;

    public CreateBusinessProfileCommandHandler(IBusinessProfileRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<BusinessProfileDto> Handle(CreateBusinessProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = new BusinessProfile
        {
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Address = request.Address
        };

        var saved = await _repository.AddAsync(profile, cancellationToken);
        return _mapper.Map<BusinessProfileDto>(saved);
    }
}

using AISA.Application.BusinessProfiles.DTOs;
using AISA.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AISA.Application.BusinessProfiles.Queries.GetMyBusinessProfile;

public class GetMyBusinessProfileQueryHandler : IRequestHandler<GetMyBusinessProfileQuery, BusinessProfileDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly IBusinessProfileRepository _profileRepository;
    private readonly IMapper _mapper;

    public GetMyBusinessProfileQueryHandler(
        IUserRepository userRepository, 
        IBusinessProfileRepository profileRepository, 
        IMapper mapper)
    {
        _userRepository = userRepository;
        _profileRepository = profileRepository;
        _mapper = mapper;
    }

    public async Task<BusinessProfileDto?> Handle(GetMyBusinessProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        
        if (user?.BusinessProfileId is null)
        {
            return null;
        }

        var profile = await _profileRepository.GetByIdAsync(user.BusinessProfileId.Value, cancellationToken);
        return _mapper.Map<BusinessProfileDto>(profile);
    }
}

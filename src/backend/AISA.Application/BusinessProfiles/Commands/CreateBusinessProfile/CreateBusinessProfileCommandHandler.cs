using AISA.Application.BusinessProfiles.DTOs;
using AISA.Domain.Entities;
using AISA.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AISA.Application.BusinessProfiles.Commands.CreateBusinessProfile;

public class CreateBusinessProfileCommandHandler : IRequestHandler<CreateBusinessProfileCommand, BusinessProfileDto>
{
    private readonly IBusinessProfileRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CreateBusinessProfileCommandHandler(
        IBusinessProfileRepository repository, 
        IUserRepository userRepository,
        IMapper mapper)
    {
        _repository = repository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<BusinessProfileDto> Handle(CreateBusinessProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = new BusinessProfile
        {
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Address = request.Address,
            GoogleMapsUrl = request.GoogleMapsUrl,
            TripAdvisorUrl = request.TripAdvisorUrl
        };

        var saved = await _repository.AddAsync(profile, cancellationToken);

        // Actualizează utilizatorul pentru a include acest profil
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is not null)
        {
            user.BusinessProfileId = saved.Id;
            await _userRepository.UpdateAsync(user);
        }

        return _mapper.Map<BusinessProfileDto>(saved);
    }
}

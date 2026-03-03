using AISA.Application.BusinessProfiles.DTOs;
using AISA.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AISA.Application.BusinessProfiles.Commands.UpdateBusinessProfile;

public class UpdateBusinessProfileCommandHandler : IRequestHandler<UpdateBusinessProfileCommand, BusinessProfileDto>
{
    private readonly IBusinessProfileRepository _repository;
    private readonly IMapper _mapper;

    public UpdateBusinessProfileCommandHandler(IBusinessProfileRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<BusinessProfileDto> Handle(UpdateBusinessProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Business profile {request.Id} not found.");

        profile.Name = request.Name;
        profile.Description = request.Description;
        profile.Category = request.Category;
        profile.Address = request.Address;
        profile.GoogleMapsUrl = request.GoogleMapsUrl;
        profile.TripAdvisorUrl = request.TripAdvisorUrl;

        await _repository.UpdateAsync(profile, cancellationToken);
        return _mapper.Map<BusinessProfileDto>(profile);
    }
}

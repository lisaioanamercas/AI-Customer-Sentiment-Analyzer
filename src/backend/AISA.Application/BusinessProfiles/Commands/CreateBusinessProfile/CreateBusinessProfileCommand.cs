using AISA.Application.BusinessProfiles.DTOs;
using MediatR;

namespace AISA.Application.BusinessProfiles.Commands.CreateBusinessProfile;

/// <summary>
/// Command pentru crearea unui profil de afacere.
/// </summary>
public record CreateBusinessProfileCommand : IRequest<BusinessProfileDto>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Category { get; init; }
    public string? Address { get; init; }
}

using AISA.Application.BusinessProfiles.DTOs;
using MediatR;

namespace AISA.Application.BusinessProfiles.Commands.UpdateBusinessProfile;

/// <summary>
/// Command pentru actualizarea unui profil de afacere existent.
/// Include câmpurile de URL pentru scraping recenzii.
/// </summary>
public record UpdateBusinessProfileCommand : IRequest<BusinessProfileDto>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Category { get; init; }
    public string? Address { get; init; }
    public string? GoogleMapsUrl { get; init; }
    public string? TripAdvisorUrl { get; init; }
}

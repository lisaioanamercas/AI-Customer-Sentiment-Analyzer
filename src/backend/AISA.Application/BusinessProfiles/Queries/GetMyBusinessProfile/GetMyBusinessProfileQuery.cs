using AISA.Application.BusinessProfiles.DTOs;
using MediatR;

namespace AISA.Application.BusinessProfiles.Queries.GetMyBusinessProfile;

/// <summary>
/// Query pentru obținerea profilului de afacere asociat utilizatorului curent.
/// </summary>
public record GetMyBusinessProfileQuery(Guid UserId) : IRequest<BusinessProfileDto?>;

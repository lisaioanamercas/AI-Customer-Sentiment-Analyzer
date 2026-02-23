using AISA.Application.BusinessProfiles.DTOs;
using MediatR;

namespace AISA.Application.BusinessProfiles.Queries.GetBusinessProfile;

/// <summary>
/// Query pentru obținerea unui profil de afacere după ID.
/// </summary>
public record GetBusinessProfileQuery(Guid Id) : IRequest<BusinessProfileDto?>;

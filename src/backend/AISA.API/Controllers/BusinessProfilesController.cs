using AISA.Application.BusinessProfiles.Commands.CreateBusinessProfile;
using AISA.Application.BusinessProfiles.DTOs;
using AISA.Application.BusinessProfiles.Queries.GetBusinessProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AISA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BusinessProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BusinessProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obține un profil de afacere după ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BusinessProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBusinessProfile(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBusinessProfileQuery(id), cancellationToken);

        if (result is null)
            return NotFound(new { message = $"Profilul cu ID-ul {id} nu a fost găsit." });

        return Ok(result);
    }

    /// <summary>
    /// Creează un profil de afacere nou.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(BusinessProfileDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBusinessProfile([FromBody] CreateBusinessProfileCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetBusinessProfile), new { id = result.Id }, result);
    }
}

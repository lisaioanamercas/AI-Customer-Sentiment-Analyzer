using AISA.Application.BusinessProfiles.Commands.CreateBusinessProfile;
using AISA.Application.BusinessProfiles.Commands.UpdateBusinessProfile;
using AISA.Application.BusinessProfiles.DTOs;
using AISA.Application.BusinessProfiles.Queries.GetBusinessProfile;
using AISA.Application.BusinessProfiles.Queries.GetMyBusinessProfile;
using System.Security.Claims;
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
    /// Obține profilul de afacere al utilizatorului curent.
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(BusinessProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyBusinessProfile(CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new GetMyBusinessProfileQuery(userId), cancellationToken);

        if (result is null)
            return NotFound(new { message = "Nu ai un profil de afacere asociat." });

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
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var commandWithUserId = command with { UserId = userId };
        var result = await _mediator.Send(commandWithUserId, cancellationToken);
        return CreatedAtAction(nameof(GetBusinessProfile), new { id = result.Id }, result);
    }

    /// <summary>
    /// Actualizează un profil de afacere existent (inclusiv URL-uri scraping).
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BusinessProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBusinessProfile(
        Guid id, [FromBody] UpdateBusinessProfileCommand command, CancellationToken cancellationToken)
    {
        // Asigurăm că ID-ul din URL corespunde cu command-ul
        var commandWithId = command with { Id = id };
        try
        {
            var result = await _mediator.Send(commandWithId, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

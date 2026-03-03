using AISA.Application.Reviews.Commands.AddReview;
using AISA.Application.Reviews.Commands.DeleteReview;
using AISA.Application.Reviews.Commands.ScrapeAndImport;
using AISA.Application.Reviews.DTOs;
using AISA.Application.Reviews.Queries.GetReviews;
using AISA.Application.Reviews.Queries.GetSentimentTrends;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AISA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obține toate recenziile unui business profile.
    /// </summary>
    [HttpGet("{businessProfileId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<ReviewDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReviews(Guid businessProfileId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetReviewsQuery(businessProfileId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Adaugă o recenzie nouă și declanșează analiza de sentiment.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddReview([FromBody] AddReviewCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetReviews), new { businessProfileId = result.BusinessProfileId }, result);
    }

    /// <summary>
    /// Șterge o recenzie.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteReview(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteReviewCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Obține trend-urile de sentiment pentru grafice.
    /// </summary>
    [HttpGet("{businessProfileId:guid}/trends")]
    [ProducesResponseType(typeof(IReadOnlyList<SentimentTrendDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSentimentTrends(
        Guid businessProfileId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var query = new GetSentimentTrendsQuery
        {
            BusinessProfileId = businessProfileId,
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Scrapează recenzii din Google/TripAdvisor și le importează cu analiză AI.
    /// Necesită ca URL-urile sursă să fie configurate în profilul afacerii.
    /// </summary>
    [HttpPost("scrape")]
    [ProducesResponseType(typeof(ScrapeResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ScrapeAndImport(
        [FromBody] ScrapeAndImportReviewsCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

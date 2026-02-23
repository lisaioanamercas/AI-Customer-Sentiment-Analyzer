using AISA.Application.Reviews.DTOs;
using MediatR;

namespace AISA.Application.Reviews.Queries.GetSentimentTrends;

/// <summary>
/// Query pentru obținerea trend-urilor de sentiment ale unui business profile.
/// Folosit pentru graficele din dashboard.
/// </summary>
public record GetSentimentTrendsQuery : IRequest<IReadOnlyList<SentimentTrendDto>>
{
    public Guid BusinessProfileId { get; init; }

    /// <summary>Data de început a perioadei (implicit: 30 zile în urmă)</summary>
    public DateTime? FromDate { get; init; }

    /// <summary>Data de sfârșit a perioadei (implicit: azi)</summary>
    public DateTime? ToDate { get; init; }
}

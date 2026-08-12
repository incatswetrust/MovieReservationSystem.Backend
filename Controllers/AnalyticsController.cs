using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs.Analytics;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    [HttpGet("revenue")]
    public async Task<ActionResult<RevenueSummaryDto>> GetRevenue(CancellationToken cancellationToken)
    {
        var summary = await analyticsService.GetRevenueSummaryAsync(cancellationToken);
        return Ok(summary);
    }

    [HttpGet("occupancy")]
    public async Task<ActionResult<IEnumerable<ShowtimeOccupancyDto>>> GetOccupancy(CancellationToken cancellationToken)
    {
        var occupancy = await analyticsService.GetShowtimeOccupancyAsync(cancellationToken);
        return Ok(occupancy);
    }

    [HttpGet("top-movies")]
    public async Task<ActionResult<IEnumerable<TopMovieDto>>> GetTopMovies(CancellationToken cancellationToken)
    {
        var topMovies = await analyticsService.GetTopMoviesAsync(cancellationToken);
        return Ok(topMovies);
    }
}

using MovieReservationSystem.Backend.DTOs.Analytics;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface IAnalyticsService
{
    Task<RevenueSummaryDto> GetRevenueSummaryAsync(CancellationToken cancellationToken);
    Task<IEnumerable<ShowtimeOccupancyDto>> GetShowtimeOccupancyAsync(CancellationToken cancellationToken);
    Task<IEnumerable<TopMovieDto>> GetTopMoviesAsync(CancellationToken cancellationToken);
}

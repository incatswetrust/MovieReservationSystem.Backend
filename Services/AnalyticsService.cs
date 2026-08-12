using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.DTOs.Analytics;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class AnalyticsService(AppDbContext context) : IAnalyticsService
{
    public async Task<RevenueSummaryDto> GetRevenueSummaryAsync(CancellationToken cancellationToken)
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        var summary = await context.Bookings
            .AsNoTracking()
            .Where(b => b.Status != "Canceled")
            .GroupBy(b => 1)
            .Select(g => new RevenueSummaryDto
            {
                TotalRevenue = g.Sum(b => b.TotalPrice),
                RevenueLast30Days = g.Where(b => b.CreatedAt >= thirtyDaysAgo).Sum(b => b.TotalPrice),
                TotalBookingsCount = g.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return summary ?? new RevenueSummaryDto
        {
            TotalRevenue = 0m,
            RevenueLast30Days = 0m,
            TotalBookingsCount = 0
        };
    }

    public async Task<IEnumerable<ShowtimeOccupancyDto>> GetShowtimeOccupancyAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        // Scope: upcoming showtimes (StartTime in the future), soonest first, capped at 100
        // so the endpoint stays cheap even with a large showtime table.
        var raw = await context.Showtimes
            .AsNoTracking()
            .Where(s => s.StartTime >= now)
            .OrderBy(s => s.StartTime)
            .Take(100)
            .Select(s => new
            {
                s.Id,
                s.MovieId,
                MovieTitle = s.Movie!.Title,
                s.HallId,
                HallName = s.Hall!.Name,
                CinemaName = s.Hall!.Cinema!.Name,
                s.StartTime,
                TotalSeatsCount = s.Hall!.Seats!.Count,
                BookedSeatsCount = s.Bookings!
                    .Where(b => b.Status != "Canceled")
                    .SelectMany(b => b.BookedSeats!)
                    .Count()
            })
            .ToListAsync(cancellationToken);

        return raw.Select(s => new ShowtimeOccupancyDto
        {
            ShowtimeId = s.Id,
            MovieId = s.MovieId,
            MovieTitle = s.MovieTitle,
            HallId = s.HallId,
            HallName = s.HallName,
            CinemaName = s.CinemaName,
            StartTime = s.StartTime,
            BookedSeatsCount = s.BookedSeatsCount,
            TotalSeatsCount = s.TotalSeatsCount,
            OccupancyPercentage = s.TotalSeatsCount == 0
                ? 0d
                : Math.Round(s.BookedSeatsCount * 100d / s.TotalSeatsCount, 2)
        });
    }

    public async Task<IEnumerable<TopMovieDto>> GetTopMoviesAsync(CancellationToken cancellationToken)
    {
        var topMovies = await context.BookedSeats
            .AsNoTracking()
            .Where(bs => bs.Booking!.Status != "Canceled")
            .GroupBy(bs => new { bs.Booking!.Showtime!.MovieId, bs.Booking.Showtime.Movie!.Title })
            .Select(g => new TopMovieDto
            {
                MovieId = g.Key.MovieId,
                Title = g.Key.Title,
                BookedSeatsCount = g.Count()
            })
            .OrderByDescending(t => t.BookedSeatsCount)
            .Take(10)
            .ToListAsync(cancellationToken);

        return topMovies;
    }
}

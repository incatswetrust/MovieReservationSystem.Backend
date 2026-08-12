using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs.Booking;
using MovieReservationSystem.Backend.Services;
using MovieReservationSystem.Backend.Tests.TestHelpers;
using Xunit;

namespace MovieReservationSystem.Backend.Tests.Services;

/// <summary>
/// Covers BookingService.CreateAsync's seat double-booking pre-check and CancelAsync's
/// seat-release behavior (Perf-3 hardening).
///
/// Uses the SQLite in-memory provider rather than EF Core InMemory: CreateAsync opens a
/// real transaction via Database.BeginTransactionAsync, which is a relational-only API that
/// the InMemory provider does not support at all (it throws). SQLite in-memory is relational
/// enough to support transactions and, as a bonus, actually enforces the unique
/// (ShowtimeId, SeatId) index the same way SQL Server would.
/// </summary>
public class BookingServiceTests : IDisposable
{
    private readonly SqliteInMemoryFixture _fixture = new();

    private BookingService CreateService() => new(_fixture.Context, DbContextFactory.CreateMapper());

    private async Task<(User user, Showtime showtime1, Showtime showtime2, Seat seat)> SeedBaseDataAsync()
    {
        var context = _fixture.Context;

        var cinema = new Cinema { Name = "Cinema 1", Address = "Downtown" };
        context.Cinemas.Add(cinema);
        await context.SaveChangesAsync();

        var hall = new Hall { Name = "Hall 1", CinemaId = cinema.Id };
        context.Halls.Add(hall);
        await context.SaveChangesAsync();

        var movie = new Movie
        {
            Title = "Test Movie",
            Duration = 120,
            ReleaseYear = 2024,
            Base64Image = string.Empty
        };
        context.Movies.Add(movie);
        await context.SaveChangesAsync();

        var showtime1 = new Showtime { HallId = hall.Id, MovieId = movie.Id, StartTime = DateTime.UtcNow.AddDays(1), Price = 10m };
        var showtime2 = new Showtime { HallId = hall.Id, MovieId = movie.Id, StartTime = DateTime.UtcNow.AddDays(2), Price = 10m };
        context.Showtimes.AddRange(showtime1, showtime2);
        await context.SaveChangesAsync();

        var seat = new Seat { HallId = hall.Id, RowLabel = "A", SeatNumber = 1 };
        context.Seats.Add(seat);
        await context.SaveChangesAsync();

        var user = new User { Username = "booker", Role = UserRole.User };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return (user, showtime1, showtime2, seat);
    }

    [Fact]
    public async Task CreateAsync_SeatAlreadyActivelyBookedOnSameShowtime_Throws()
    {
        var (user, showtime1, _, seat) = await SeedBaseDataAsync();
        var service = CreateService();

        await service.CreateAsync(new BookingCreateDto { UserId = user.Id, ShowtimeId = showtime1.Id, SeatIds = [seat.Id] }, CancellationToken.None);

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            service.CreateAsync(new BookingCreateDto { UserId = user.Id, ShowtimeId = showtime1.Id, SeatIds = [seat.Id] }, CancellationToken.None));

        Assert.Contains("already booked", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_SameSeatOnDifferentShowtime_Succeeds()
    {
        var (user, showtime1, showtime2, seat) = await SeedBaseDataAsync();
        var service = CreateService();

        await service.CreateAsync(new BookingCreateDto { UserId = user.Id, ShowtimeId = showtime1.Id, SeatIds = [seat.Id] }, CancellationToken.None);

        var secondBooking = await service.CreateAsync(
            new BookingCreateDto { UserId = user.Id, ShowtimeId = showtime2.Id, SeatIds = [seat.Id] }, CancellationToken.None);

        Assert.Equal(showtime2.Id, secondBooking.ShowtimeId);
        Assert.Contains(seat.Id, secondBooking.SeatIds);
    }

    [Fact]
    public async Task CreateAsync_SeatWhosePriorBookingWasCanceled_Succeeds()
    {
        var (user, showtime1, _, seat) = await SeedBaseDataAsync();
        var service = CreateService();

        var firstBooking = await service.CreateAsync(
            new BookingCreateDto { UserId = user.Id, ShowtimeId = showtime1.Id, SeatIds = [seat.Id] }, CancellationToken.None);

        var canceled = await service.CancelAsync(firstBooking.Id, CancellationToken.None);
        Assert.True(canceled);

        // Cancel should have removed the BookedSeat row entirely (not just flagged the
        // booking), freeing the seat for a brand new booking on the same showtime.
        var secondBooking = await service.CreateAsync(
            new BookingCreateDto { UserId = user.Id, ShowtimeId = showtime1.Id, SeatIds = [seat.Id] }, CancellationToken.None);

        Assert.Contains(seat.Id, secondBooking.SeatIds);
        Assert.Equal(0, await _fixture.Context.BookedSeats.CountAsync(bs => bs.BookingId == firstBooking.Id));
    }

    [Fact]
    public async Task UniqueIndex_ConcurrentInsertBypassingAppLevelCheck_ThrowsDbUpdateException()
    {
        // This simulates the exact race BookingService.CreateAsync's app-level pre-check
        // cannot fully close: two bookings for the same seat + showtime, inserted without
        // going through the "is this seat already booked" query first (as if two requests
        // both passed that check concurrently). The DB-level unique index on
        // BookedSeat(ShowtimeId, SeatId) — added this session — must be the backstop.
        var (user, showtime1, _, seat) = await SeedBaseDataAsync();
        var context = _fixture.Context;

        var booking1 = new Booking { UserId = user.Id, ShowtimeId = showtime1.Id, Status = "Active", TotalPrice = 10m };
        var booking2 = new Booking { UserId = user.Id, ShowtimeId = showtime1.Id, Status = "Active", TotalPrice = 10m };
        context.Bookings.AddRange(booking1, booking2);
        await context.SaveChangesAsync();

        context.BookedSeats.Add(new BookedSeat { BookingId = booking1.Id, ShowtimeId = showtime1.Id, SeatId = seat.Id, Price = 10m });
        await context.SaveChangesAsync();

        context.BookedSeats.Add(new BookedSeat { BookingId = booking2.Id, ShowtimeId = showtime1.Id, SeatId = seat.Id, Price = 10m });

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    public void Dispose() => _fixture.Dispose();
}

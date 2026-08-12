using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.Tests.TestHelpers;
using Xunit;

namespace MovieReservationSystem.Backend.Tests.Data;

/// <summary>
/// Covers the OnModelCreating changes made this session: Booking.UserId's FK is now
/// DeleteBehavior.Restrict (was the EF Core default of Cascade), so deleting a user with
/// existing bookings must fail loudly instead of silently wiping their booking history.
///
/// Uses SQLite in-memory: the EF Core InMemory provider does not model relational delete
/// behaviors like Restrict/Cascade at all (it just leaves FK values as-is), so it cannot
/// catch a regression here — only a relational provider can.
/// </summary>
public class AppDbContextTests : IDisposable
{
    private readonly SqliteInMemoryFixture _fixture = new();

    [Fact]
    public async Task DeletingUser_WithExistingBooking_ThrowsInsteadOfCascading()
    {
        var context = _fixture.Context;

        var cinema = new Cinema { Name = "Cinema 1" };
        context.Cinemas.Add(cinema);
        await context.SaveChangesAsync();

        var hall = new Hall { Name = "Hall 1", CinemaId = cinema.Id };
        context.Halls.Add(hall);
        await context.SaveChangesAsync();

        var movie = new Movie { Title = "Test Movie", Duration = 100, ReleaseYear = 2024, Base64Image = string.Empty };
        context.Movies.Add(movie);
        await context.SaveChangesAsync();

        var showtime = new Showtime { HallId = hall.Id, MovieId = movie.Id, StartTime = DateTime.UtcNow, Price = 10m };
        context.Showtimes.Add(showtime);
        await context.SaveChangesAsync();

        var user = new User { Username = "hasbookings", Role = UserRole.User };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var booking = new Booking { UserId = user.Id, ShowtimeId = showtime.Id, Status = "Active" };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Deleting the user from a context that already has their booking loaded/tracked is
        // caught by EF Core's change tracker itself, synchronously, the moment Remove() is
        // called (a required relationship would be severed) — it never even gets as far as
        // issuing SQL. That's a real guard too, so assert it here...
        Assert.Throws<InvalidOperationException>(() => context.Users.Remove(user));

        // ...but the thing this test is really meant to prove is that the *database* itself
        // (via the Restrict-mapped FK) rejects the delete, independent of what the change
        // tracker happens to have loaded. Redo the delete from a second, untracked context on
        // the same underlying database so EF has no client-side knowledge of the booking and
        // must round-trip to SQLite, which enforces the FK constraint and reports it back as
        // a DbUpdateException.
        using var freshContext = _fixture.CreateAdditionalContext();
        var untrackedUser = await freshContext.Users.FindAsync(user.Id);
        Assert.NotNull(untrackedUser);
        freshContext.Users.Remove(untrackedUser);

        await Assert.ThrowsAsync<DbUpdateException>(() => freshContext.SaveChangesAsync());

        // The user (and their booking) must still exist — the delete must have been rejected,
        // not partially applied.
        Assert.NotNull(await context.Users.FindAsync(user.Id));
        Assert.NotNull(await context.Bookings.FindAsync(booking.Id));
    }

    [Fact]
    public async Task DeletingUser_WithNoBookings_Succeeds()
    {
        var context = _fixture.Context;

        var user = new User { Username = "nobookings", Role = UserRole.User };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        context.Users.Remove(user);
        await context.SaveChangesAsync();

        Assert.Null(await context.Users.FindAsync(user.Id));
    }

    public void Dispose() => _fixture.Dispose();
}

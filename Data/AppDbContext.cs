using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Domain;

namespace MovieReservationSystem.Backend.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies { get; set; } = null!;
    public DbSet<Cinema> Cinemas { get; set; } = null!;
    public DbSet<Hall> Halls { get; set; } = null!;
    public DbSet<Showtime> Showtimes { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<BookedSeat> BookedSeats { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Seat> Seats { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<HallImage> HallImages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(rt => rt.Token)
            .IsUnique();

        // Deleting a user must not silently wipe their booking history — EF Core's default
        // behavior for a required FK like Booking.UserId is cascade delete, which would do
        // exactly that. Block the delete instead; callers must handle/relocate a user's
        // bookings first (or use a soft-delete) before removing the account.
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Safety net under the application-level "is this seat already booked" check in
        // BookingService.CreateAsync, which is vulnerable to a race between two concurrent
        // bookings for the same seat. Cancelling a booking removes its BookedSeat rows (see
        // BookingService.CancelAsync), so this only ever constrains currently-held seats.
        modelBuilder.Entity<BookedSeat>()
            .HasIndex(bs => new { bs.ShowtimeId, bs.SeatId })
            .IsUnique();
    }
}
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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
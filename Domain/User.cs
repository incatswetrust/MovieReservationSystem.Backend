using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Backend.Domain;

public enum UserRole
{
    User,
    Admin,
    Viewer
}

public class User
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Username { get; set; } = null!;
    public string? PasswordHash { get; set; }

    [MaxLength(320)]
    public string? Email { get; set; }

    [MaxLength(100)]
    public string? GoogleId { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public ICollection<Booking>? Bookings { get; set; }
}

namespace MovieReservationSystem.Backend.Domain;

public enum UserRole
{
    User,
    Admin
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string? PasswordHash { get; set; }
    public string? Email { get; set; }
    public string? GoogleId { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public ICollection<Booking>? Bookings { get; set; }
}
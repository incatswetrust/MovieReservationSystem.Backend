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
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; } = UserRole.User;
    public ICollection<Booking>? Bookings { get; set; }
}
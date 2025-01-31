using MovieReservationSystem.Backend.Domain;

namespace MovieReservationSystem.Backend.DTOs.User;

public class UserRegisterDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRole Role { get; set; }
}
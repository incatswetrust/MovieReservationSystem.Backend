using MovieReservationSystem.Backend.Domain;

namespace MovieReservationSystem.Backend.DTOs.User;

public class UserLoginDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
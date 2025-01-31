using MovieReservationSystem.Backend.Domain;

namespace MovieReservationSystem.Backend.DTOs.User;

public class UserReadDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public UserRole Role { get; set; }
}
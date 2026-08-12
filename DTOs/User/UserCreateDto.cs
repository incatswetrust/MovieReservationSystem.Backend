using System.ComponentModel.DataAnnotations;
using MovieReservationSystem.Backend.Domain;

namespace MovieReservationSystem.Backend.DTOs.User;

public class UserCreateDto
{
    [Required, MinLength(3), MaxLength(100)]
    public string Username { get; set; } = null!;

    [Required, MinLength(8)]
    public string Password { get; set; } = null!;

    [EmailAddress, MaxLength(320)]
    public string? Email { get; set; }

    public UserRole Role { get; set; }
}

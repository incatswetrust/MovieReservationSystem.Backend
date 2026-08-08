using System.ComponentModel.DataAnnotations;
using MovieReservationSystem.Backend.Domain;

namespace MovieReservationSystem.Backend.DTOs.User;

public class UserRegisterDto
{
    [Required, MinLength(3)]
    public string Username { get; set; } = null!;

    [Required, MinLength(8)]
    public string Password { get; set; } = null!;

    [EmailAddress]
    public string? Email { get; set; }

    public UserRole Role { get; set; }
}
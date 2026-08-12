using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Backend.DTOs.User;

public class UserRegisterDto
{
    [Required, MinLength(3), MaxLength(100)]
    public string Username { get; set; } = null!;

    [Required, MinLength(8)]
    public string Password { get; set; } = null!;

    [EmailAddress, MaxLength(320)]
    public string? Email { get; set; }
}
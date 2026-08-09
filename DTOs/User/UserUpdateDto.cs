using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Backend.DTOs.User;

public class UserUpdateDto
{
    [Required, MinLength(3)]
    public string Username { get; set; } = null!;

    [EmailAddress]
    public string? Email { get; set; }
}

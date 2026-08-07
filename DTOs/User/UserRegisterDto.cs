using System.ComponentModel.DataAnnotations;
using MovieReservationSystem.Backend.Domain;

namespace MovieReservationSystem.Backend.DTOs.User;

public class UserRegisterDto
{
    [Required, MinLength(3)]
    public string Username { get; set; } = null!;

    [Required, MinLength(8)]
    public string Password { get; set; } = null!;

    public UserRole Role { get; set; }
}
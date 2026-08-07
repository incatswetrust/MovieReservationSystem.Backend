using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Backend.DTOs.Cinema;

public class CinemaCreateDto
{
    [Required]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Address { get; set; }
}
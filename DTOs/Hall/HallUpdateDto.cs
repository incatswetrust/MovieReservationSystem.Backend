using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Backend.DTOs.Hall;

public class HallUpdateDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = null!;
    public int CinemaId { get; set; }
}

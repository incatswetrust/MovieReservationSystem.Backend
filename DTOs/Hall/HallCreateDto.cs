using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Backend.DTOs.Hall;

public class HallCreateDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required, Range(1, int.MaxValue)]
    public int CinemaId { get; set; }

    [Range(1, 50)]
    public int NumberOfRows { get; set; }

    [Range(1, 50)]
    public int SeatsPerRow { get; set; }
}

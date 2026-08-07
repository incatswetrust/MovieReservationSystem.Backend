using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Backend.DTOs.Showtime;

public class ShowtimeCreateDto
{
    [Required, Range(1, int.MaxValue)]
    public int HallId { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int MovieId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Range(0, 10000)]
    public decimal Price { get; set; }
}
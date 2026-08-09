using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Backend.DTOs.HallImage;

public class HallImageCreateDto
{
    [Required]
    public string ImageUrl { get; set; } = null!;

    public int Order { get; set; }

    public string? Caption { get; set; }
}

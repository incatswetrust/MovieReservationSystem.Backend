using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Backend.DTOs.User;

public class GoogleExchangeDto
{
    [Required]
    public string Code { get; set; } = null!;
}

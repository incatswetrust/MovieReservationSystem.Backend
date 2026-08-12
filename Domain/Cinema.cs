using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Backend.Domain;

public class Cinema
{
    public int Id { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Address { get; set; }
    public ICollection<Hall>? Halls { get; set; }
}

namespace MovieReservationSystem.Backend.DTOs.Cinema;

public class CinemaCreateDto
{
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
}
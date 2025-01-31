namespace MovieReservationSystem.Backend.DTOs.Cinema;

public class CinemaReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
}
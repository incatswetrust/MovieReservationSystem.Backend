namespace MovieReservationSystem.Backend.DTOs.Hall;

public class HallUpdateDto
{
    public string Name { get; set; } = null!;
    public int CinemaId { get; set; }
}
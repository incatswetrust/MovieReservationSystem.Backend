namespace MovieReservationSystem.Backend.DTOs.Hall;

public class HallCreateDto
{
    public string Name { get; set; } = null!;
    public int CinemaId { get; set; }
    public int NumberOfRows { get; set; }
    public int SeatsPerRow { get; set; }
}
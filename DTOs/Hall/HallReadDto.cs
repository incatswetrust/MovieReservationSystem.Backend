using MovieReservationSystem.Backend.DTOs.Seat;

namespace MovieReservationSystem.Backend.DTOs.Hall;

public class HallReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CinemaId { get; set; }
    public List<SeatReadDto>? Seats { get; set; }
}
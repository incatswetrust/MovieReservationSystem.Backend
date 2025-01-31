namespace MovieReservationSystem.Backend.DTOs.Showtime;

public class ShowtimeReadDto
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public int MovieId { get; set; }
    public DateTime StartTime { get; set; }
    public decimal Price { get; set; }
}
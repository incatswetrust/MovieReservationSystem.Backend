namespace MovieReservationSystem.Backend.DTOs.Seat;

public class SeatReadDto
{
    public int Id { get; set; }
    public string RowLabel { get; set; } = null!;
    public int SeatNumber { get; set; }
    public bool IsReserved { get; set; }
}
namespace MovieReservationSystem.Backend.Domain;

public class Seat
{
    public int Id { get; set; }
    public int HallId { get; set; }

    public string RowLabel { get; set; } = null!; 
    public int SeatNumber { get; set; }

    public Hall? Hall { get; set; }
}
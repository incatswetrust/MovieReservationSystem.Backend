namespace MovieReservationSystem.Backend.DTOs.Booking;

public class BookingReadDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ShowtimeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalPrice { get; set; }
    public List<int> SeatIds { get; set; } = new();
}
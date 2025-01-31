namespace MovieReservationSystem.Backend.DTOs.Booking;

public class BookingCreateDto
{
    public int UserId { get; set; }
    public int ShowtimeId { get; set; }
    public List<int> SeatIds { get; set; } = new();
}
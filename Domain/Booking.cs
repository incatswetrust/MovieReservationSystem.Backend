namespace MovieReservationSystem.Backend.Domain;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int ShowtimeId { get; set; }
    public Showtime? Showtime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Active";
    public decimal TotalPrice { get; set; }
    public ICollection<BookedSeat>? BookedSeats { get; set; }
}
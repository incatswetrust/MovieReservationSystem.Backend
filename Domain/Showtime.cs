namespace MovieReservationSystem.Backend.Domain;

public class Showtime
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public Hall? Hall { get; set; }
    public int MovieId { get; set; }
    public Movie? Movie { get; set; }
    public DateTime StartTime { get; set; }
    public decimal Price { get; set; }
    public ICollection<Booking>? Bookings { get; set; }
}
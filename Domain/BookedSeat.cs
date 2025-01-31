namespace MovieReservationSystem.Backend.Domain;

public class BookedSeat
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }
    public int SeatId { get; set; }
    public decimal Price { get; set; }
}
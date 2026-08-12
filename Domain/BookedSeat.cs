namespace MovieReservationSystem.Backend.Domain;

public class BookedSeat
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }
    public int SeatId { get; set; }

    // Denormalized from Booking.ShowtimeId so a unique (ShowtimeId, SeatId) index can
    // guarantee at the database level that the same seat is never double-booked for the
    // same showtime, even under concurrent requests racing past the application-level
    // pre-check in BookingService.CreateAsync.
    public int ShowtimeId { get; set; }
    public decimal Price { get; set; }
}

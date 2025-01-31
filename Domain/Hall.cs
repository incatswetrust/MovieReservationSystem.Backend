namespace MovieReservationSystem.Backend.Domain;

public class Hall
{
    public int Id { get; set; }
        
    public string Name { get; set; } = null!;
    public int CinemaId { get; set; }
    public Cinema? Cinema { get; set; }
    public ICollection<Showtime>? Showtimes { get; set; }
    public ICollection<Seat>? Seats { get; set; }
}
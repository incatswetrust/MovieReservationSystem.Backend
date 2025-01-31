namespace MovieReservationSystem.Backend.Domain;

public class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = null!; 
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public int Duration { get; set; } 
    public int ReleaseYear { get; set; }
    public ICollection<Showtime>? Showtimes { get; set; }
}


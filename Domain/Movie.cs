namespace MovieReservationSystem.Backend.Domain;

public class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public int Duration { get; set; }
    public int ReleaseYear { get; set; }
    public string Base64Image { get; set; }
    public string? Director { get; set; }
    public string? Cast { get; set; }
    public string? Language { get; set; }
    public string? Country { get; set; }
    public string? AgeRating { get; set; } // "0+", "6+", "12+", "16+", "18+"
    public string? TrailerUrl { get; set; }
    public decimal? ImdbRating { get; set; } // 0.0-10.0
    public string? PosterUrl { get; set; }
    public ICollection<Showtime>? Showtimes { get; set; }
}


namespace MovieReservationSystem.Backend.DTOs.Movie;

public class MovieUpdateDto
{
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
    public string? AgeRating { get; set; }
    public string? TrailerUrl { get; set; }
    public decimal? ImdbRating { get; set; }
    public string? PosterUrl { get; set; }
}
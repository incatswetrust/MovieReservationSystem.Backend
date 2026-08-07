using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Backend.DTOs.Movie;

public class MovieCreateDto
{
    [Required]
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Genre { get; set; }

    [Range(1, 600)]
    public int Duration { get; set; }  // Minutes

    [Range(1900, 2100)]
    public int ReleaseYear { get; set; }
    public string Base64Image { get; set; }
    public string? Director { get; set; }
    public string? Cast { get; set; }
    public string? Language { get; set; }
    public string? Country { get; set; }
    public string? AgeRating { get; set; }
    public string? TrailerUrl { get; set; }

    [Range(0.0, 10.0)]
    public decimal? ImdbRating { get; set; }
    public string? PosterUrl { get; set; }
}
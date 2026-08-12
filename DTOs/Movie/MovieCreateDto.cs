using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Backend.DTOs.Movie;

public class MovieCreateDto
{
    [Required, MaxLength(300)]
    public string Title { get; set; } = null!;

    [MaxLength(4000)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Genre { get; set; }

    [Range(1, 600)]
    public int Duration { get; set; }  // Minutes

    [Range(1900, 2100)]
    public int ReleaseYear { get; set; }
    public string Base64Image { get; set; }

    [MaxLength(200)]
    public string? Director { get; set; }

    [MaxLength(2000)]
    public string? Cast { get; set; }

    [MaxLength(100)]
    public string? Language { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(20)]
    public string? AgeRating { get; set; }

    [MaxLength(500)]
    public string? TrailerUrl { get; set; }

    [Range(0.0, 10.0)]
    public decimal? ImdbRating { get; set; }

    [MaxLength(2000)]
    public string? PosterUrl { get; set; }
}

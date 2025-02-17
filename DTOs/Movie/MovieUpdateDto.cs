namespace MovieReservationSystem.Backend.DTOs.Movie;

public class MovieUpdateDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public int Duration { get; set; }
    public int ReleaseYear { get; set; }
    public string Base64Image { get; set; }
}
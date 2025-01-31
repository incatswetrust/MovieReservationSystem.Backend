namespace MovieReservationSystem.Backend.DTOs.Movie;

public class MovieReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public int Duration { get; set; }
    public int ReleaseYear { get; set; }
}
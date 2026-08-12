namespace MovieReservationSystem.Backend.DTOs.Analytics;

public class TopMovieDto
{
    public int MovieId { get; set; }
    public string Title { get; set; } = null!;
    public int BookedSeatsCount { get; set; }
}

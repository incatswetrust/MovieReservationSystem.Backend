namespace MovieReservationSystem.Backend.DTOs.Analytics;

public class ShowtimeOccupancyDto
{
    public int ShowtimeId { get; set; }
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = null!;
    public int HallId { get; set; }
    public string HallName { get; set; } = null!;
    public string CinemaName { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public int BookedSeatsCount { get; set; }
    public int TotalSeatsCount { get; set; }
    public double OccupancyPercentage { get; set; }
}

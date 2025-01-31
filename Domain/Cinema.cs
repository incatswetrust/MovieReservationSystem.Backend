namespace MovieReservationSystem.Backend.Domain;

public class Cinema
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public ICollection<Hall>? Halls { get; set; }
}
namespace MovieReservationSystem.Backend.Domain;

public class HallImage
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public Hall? Hall { get; set; }
    public string ImageUrl { get; set; } = null!;
    public int Order { get; set; }
    public string? Caption { get; set; }
}

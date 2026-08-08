namespace MovieReservationSystem.Backend.DTOs.HallImage;

public class HallImageReadDto
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public int Order { get; set; }
    public string? Caption { get; set; }
}

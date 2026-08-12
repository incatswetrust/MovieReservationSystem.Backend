namespace MovieReservationSystem.Backend.DTOs.HallImage;

public class HallImageReadDto
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public string? Url { get; set; }
    public string? Base64Image { get; set; }
    public int Order { get; set; }
    public string? Caption { get; set; }
}

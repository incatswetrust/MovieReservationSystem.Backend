namespace MovieReservationSystem.Backend.DTOs.HallImage;

public class HallImageCreateDto
{
    public string? Url { get; set; }
    public string? Base64Image { get; set; }

    public int Order { get; set; }

    public string? Caption { get; set; }
}

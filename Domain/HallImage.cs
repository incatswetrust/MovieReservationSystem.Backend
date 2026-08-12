namespace MovieReservationSystem.Backend.Domain;

public class HallImage
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public Hall? Hall { get; set; }

    // Either an external image URL or a compressed data-URI uploaded from the browser
    // (same base64-in-DB approach as Movie.Base64Image) — at least one is required, enforced
    // in HallImageService rather than via a single [Required] DTO property, since neither
    // field is individually mandatory.
    public string? Url { get; set; }
    public string? Base64Image { get; set; }
    public int Order { get; set; }
    public string? Caption { get; set; }
}

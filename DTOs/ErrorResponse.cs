namespace MovieReservationSystem.Backend.DTOs;

public class ErrorResponse
{
    public string Error { get; set; }
    public object? Details { get; set; }

    public ErrorResponse(string error, object? details = null)
    {
        Error = error;
        Details = details;
    }
}

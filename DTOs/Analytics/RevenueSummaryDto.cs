namespace MovieReservationSystem.Backend.DTOs.Analytics;

public class RevenueSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public decimal RevenueLast30Days { get; set; }
    public int TotalBookingsCount { get; set; }
}

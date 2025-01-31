using MovieReservationSystem.Backend.DTOs.Seat;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface ISeatService
{
    Task<IEnumerable<SeatReadDto>> GetSeatsByShowtimeAsync(int showtimeId);
}
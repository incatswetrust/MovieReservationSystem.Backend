using MovieReservationSystem.Backend.DTOs.Showtime;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface IShowtimeService
{
    Task<IEnumerable<ShowtimeReadDto>> GetAllAsync();
    Task<ShowtimeReadDto?> GetByIdAsync(int id);
    Task<ShowtimeReadDto> CreateAsync(ShowtimeCreateDto dto);
    Task<ShowtimeReadDto?> UpdateAsync(int id, ShowtimeUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<ShowtimeReadDto>> GetByMovieIdAsync(int movieId);
    Task<IEnumerable<ShowtimeReadDto>> GetByHallIdAsync(int hallId);
}
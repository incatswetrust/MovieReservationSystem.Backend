using MovieReservationSystem.Backend.DTOs.Showtime;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface IShowtimeService
{
    Task<IEnumerable<ShowtimeReadDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<ShowtimeReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ShowtimeReadDto> CreateAsync(ShowtimeCreateDto dto, CancellationToken cancellationToken);
    Task<ShowtimeReadDto?> UpdateAsync(int id, ShowtimeUpdateDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<ShowtimeReadDto>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken);
    Task<IEnumerable<ShowtimeReadDto>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken);
    Task<IEnumerable<ShowtimeReadDto>> GetAvailableAsync(DateOnly date, CancellationToken cancellationToken);
}

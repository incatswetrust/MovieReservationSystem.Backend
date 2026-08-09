using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.DTOs.Movie;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface IMovieService
{
    Task<IEnumerable<MovieReadDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<PagedResult<MovieReadDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<MovieReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<MovieReadDto> CreateAsync(MovieCreateDto dto, CancellationToken cancellationToken);
    Task<MovieReadDto?> UpdateAsync(int id, MovieUpdateDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<MovieReadDto>> GetByGenreAsync(string genre, CancellationToken cancellationToken);
}

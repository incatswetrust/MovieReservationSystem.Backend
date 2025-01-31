using MovieReservationSystem.Backend.DTOs.Movie;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface IMovieService
{
    Task<IEnumerable<MovieReadDto>> GetAllAsync();
    Task<MovieReadDto?> GetByIdAsync(int id);
    Task<MovieReadDto> CreateAsync(MovieCreateDto dto);
    Task<MovieReadDto?> UpdateAsync(int id, MovieUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<MovieReadDto>> GetByGenreAsync(string genre);
}
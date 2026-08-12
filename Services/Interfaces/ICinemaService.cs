using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.DTOs.Cinema;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface ICinemaService
{
    Task<IEnumerable<CinemaReadDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<PagedResult<CinemaReadDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<CinemaReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<CinemaReadDto> CreateAsync(CinemaCreateDto dto, CancellationToken cancellationToken);
    Task<CinemaReadDto?> UpdateAsync(int id, CinemaUpdateDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<CinemaReadDto>> SearchAsync(string q, CancellationToken cancellationToken);
}

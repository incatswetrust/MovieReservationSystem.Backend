using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.DTOs.Hall;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface IHallService
{
    Task<IEnumerable<HallReadDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<PagedResult<HallReadDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<HallReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<HallReadDto> CreateAsync(HallCreateDto dto, CancellationToken cancellationToken);
    Task<HallReadDto?> UpdateAsync(int id, HallUpdateDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<HallReadDto>> GetByCinemaId(int cinemaId, CancellationToken cancellationToken);
    Task<IEnumerable<HallReadDto>> SearchAsync(string q, CancellationToken cancellationToken);

    /// <summary>
    /// Evicts the cached GetByIdAsync entry for the given hall id.
    /// Callers that mutate a hall's related data (e.g. hall images) without going through
    /// CreateAsync/UpdateAsync/DeleteAsync must call this explicitly to avoid serving stale data.
    /// </summary>
    void InvalidateHallCache(int id);
}

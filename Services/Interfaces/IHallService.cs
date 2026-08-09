using MovieReservationSystem.Backend.DTOs.Hall;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface IHallService
{
    Task<IEnumerable<HallReadDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<HallReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<HallReadDto> CreateAsync(HallCreateDto dto, CancellationToken cancellationToken);
    Task<HallReadDto?> UpdateAsync(int id, HallUpdateDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<HallReadDto>> GetByCinemaId(int cinemaId, CancellationToken cancellationToken);
}

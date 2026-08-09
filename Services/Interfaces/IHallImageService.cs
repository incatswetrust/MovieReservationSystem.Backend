using MovieReservationSystem.Backend.DTOs.HallImage;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface IHallImageService
{
    Task<IEnumerable<HallImageReadDto>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken);
    Task<HallImageReadDto> CreateAsync(int hallId, HallImageCreateDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int hallId, int imageId, CancellationToken cancellationToken);
}

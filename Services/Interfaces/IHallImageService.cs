using MovieReservationSystem.Backend.DTOs.HallImage;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface IHallImageService
{
    Task<IEnumerable<HallImageReadDto>> GetByHallIdAsync(int hallId);
    Task<HallImageReadDto> CreateAsync(int hallId, HallImageCreateDto dto);
    Task<bool> DeleteAsync(int hallId, int imageId);
}

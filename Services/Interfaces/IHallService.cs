using MovieReservationSystem.Backend.DTOs.Hall;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface IHallService
{
    Task<IEnumerable<HallReadDto>> GetAllAsync();
    Task<HallReadDto?> GetByIdAsync(int id);
    Task<HallReadDto> CreateAsync(HallCreateDto dto);
    Task<HallReadDto?> UpdateAsync(int id, HallUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
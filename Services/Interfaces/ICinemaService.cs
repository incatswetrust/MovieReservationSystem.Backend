using MovieReservationSystem.Backend.DTOs.Cinema;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface ICinemaService
{
    Task<IEnumerable<CinemaReadDto>> GetAllAsync();
    Task<CinemaReadDto?> GetByIdAsync(int id);
    Task<CinemaReadDto> CreateAsync(CinemaCreateDto dto);
    Task<CinemaReadDto?> UpdateAsync(int id, CinemaUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
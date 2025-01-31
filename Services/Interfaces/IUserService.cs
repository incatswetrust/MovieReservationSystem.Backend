using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs.User;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface IUserService
{
    string GenerateJwtToken(UserReadDto user, string secretKey);
    Task<UserReadDto> RegisterAsync(UserRegisterDto dto);
    Task<UserReadDto?> LoginAsync(UserLoginDto dto);
    Task<IEnumerable<UserReadDto>> GetAllAsync();
    Task<UserReadDto?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
}
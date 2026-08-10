using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs.User;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface IUserService
{
    string GenerateJwtToken(UserReadDto user, string secretKey);
    Task<(UserReadDto User, string RefreshToken)> RegisterAsync(UserRegisterDto dto, CancellationToken cancellationToken);
    Task<(UserReadDto User, string RefreshToken)?> LoginAsync(UserLoginDto dto, CancellationToken cancellationToken);
    Task<IEnumerable<UserReadDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<UserReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<string> GenerateRefreshTokenAsync(int userId, CancellationToken cancellationToken);
    Task<(UserReadDto User, string RefreshToken)?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<(UserReadDto User, string RefreshToken)> FindOrCreateGoogleUserAsync(string email, string googleId, CancellationToken cancellationToken);
    Task<UserReadDto?> UpdateProfileAsync(int userId, UserUpdateDto dto, CancellationToken cancellationToken);
    Task<UserReadDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken);
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs.User;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class UserService(AppDbContext context, IMapper mapper) : IUserService
{
        public async Task<(UserReadDto User, string RefreshToken)> RegisterAsync(UserRegisterDto dto)
        {
            var existingUser = await context.Users
                .AnyAsync(u => u.Username == dto.Username);

            if (existingUser)
            {
                throw new Exception("Username already taken.");
            }
            var user = mapper.Map<User>(dto);
            user.PasswordHash = HashPassword(dto.Password);
            context.Users.Add(user);
            await context.SaveChangesAsync();
            var userReadDto = mapper.Map<UserReadDto>(user);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);
            return (userReadDto, refreshToken);
        }
        public async Task<(UserReadDto User, string RefreshToken)?> LoginAsync(UserLoginDto dto)
        {
            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null || user.PasswordHash == null) return null;
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

            var userReadDto = mapper.Map<UserReadDto>(user);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);
            return (userReadDto, refreshToken);
        }
        public async Task<IEnumerable<UserReadDto>> GetAllAsync()
        {
            var users = await context.Users.AsNoTracking().ToListAsync();
            return mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        public async Task<UserReadDto?> GetByIdAsync(int id)
        {
            var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            return user == null ? null : mapper.Map<UserReadDto>(user);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null) return false;

            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return true;
        }
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            context.RefreshTokens.Add(new RefreshToken
            {
                Token = token,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });
            await context.SaveChangesAsync();
            return token;
        }

        public async Task<(UserReadDto User, string RefreshToken)?> RefreshTokenAsync(string refreshToken)
        {
            var existing = await context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (existing?.User == null || existing.IsRevoked || existing.ExpiresAt < DateTime.UtcNow)
                return null;

            existing.IsRevoked = true;
            var newToken = await GenerateRefreshTokenAsync(existing.UserId);

            var userReadDto = mapper.Map<UserReadDto>(existing.User);
            return (userReadDto, newToken);
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var existing = await context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            if (existing == null) return;

            existing.IsRevoked = true;
            await context.SaveChangesAsync();
        }

        public async Task<(UserReadDto User, string RefreshToken)> FindOrCreateGoogleUserAsync(string email, string googleId)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.GoogleId == googleId || u.Email == email);

            if (user == null)
            {
                user = new User
                {
                    Username = email,
                    Email = email,
                    GoogleId = googleId,
                    Role = UserRole.User
                };
                context.Users.Add(user);
            }
            else if (user.GoogleId == null)
            {
                user.GoogleId = googleId;
            }

            await context.SaveChangesAsync();

            var userReadDto = mapper.Map<UserReadDto>(user);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);
            return (userReadDto, refreshToken);
        }

        public string GenerateJwtToken(UserReadDto user, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()) // Enum -> string
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes), 
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
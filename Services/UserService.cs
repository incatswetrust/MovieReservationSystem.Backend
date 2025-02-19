using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs.User;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class UserService(AppDbContext context, IMapper mapper) : IUserService
{
        public async Task<UserReadDto> RegisterAsync(UserRegisterDto dto)
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
            return userReadDto;
        }
        public async Task<UserReadDto?> LoginAsync(UserLoginDto dto)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null) return null;
            var hash = HashPassword(dto.Password);
            return user.PasswordHash != hash ? null : mapper.Map<UserReadDto>(user);
        }
        public async Task<IEnumerable<UserReadDto>> GetAllAsync()
        {
            var users = await context.Users.ToListAsync();
            return mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        public async Task<UserReadDto?> GetByIdAsync(int id)
        {
            var user = await context.Users.FindAsync(id);
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
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha.ComputeHash(bytes);
            return Convert.ToHexString(hashBytes);
        }
        
        public async Task<UserReadDto> Test(UserRegisterDto dto)
        {
            var existingUser = await context.Users
                .AnyAsync(u => u.Username == dto.Username);

            if (existingUser)
            {
                throw new Exception("Username already taken.");
            }
            var user = mapper.Map<User>(dto);
            user.Role = UserRole.Admin;
            user.PasswordHash = HashPassword(dto.Password);
            context.Users.Add(user);
            await context.SaveChangesAsync();
            var userReadDto = mapper.Map<UserReadDto>(user);
            return userReadDto;
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
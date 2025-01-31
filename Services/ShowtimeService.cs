using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs.Showtime;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class ShowtimeService(AppDbContext context, IMapper mapper) : IShowtimeService
{
    public async Task<IEnumerable<ShowtimeReadDto>> GetAllAsync()
        {
            var showtimes = await context.Showtimes.ToListAsync();
            return mapper.Map<IEnumerable<ShowtimeReadDto>>(showtimes);
        }

        public async Task<ShowtimeReadDto?> GetByIdAsync(int id)
        {
            var showtime = await context.Showtimes.FindAsync(id);
            if (showtime == null) return null;

            return mapper.Map<ShowtimeReadDto>(showtime);
        }

        public async Task<ShowtimeReadDto> CreateAsync(ShowtimeCreateDto dto)
        {
            var showtime = mapper.Map<Showtime>(dto);
            context.Showtimes.Add(showtime);
            await context.SaveChangesAsync();

            return mapper.Map<ShowtimeReadDto>(showtime);
        }

        public async Task<ShowtimeReadDto?> UpdateAsync(int id, ShowtimeUpdateDto dto)
        {
            var showtime = await context.Showtimes.FindAsync(id);
            if (showtime == null) return null;

            mapper.Map(dto, showtime);
            await context.SaveChangesAsync();

            return mapper.Map<ShowtimeReadDto>(showtime);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var showtime = await context.Showtimes.FindAsync(id);
            if (showtime == null) return false;

            context.Showtimes.Remove(showtime);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<ShowtimeReadDto>> GetByMovieIdAsync(int movieId)
        {
            var showtimes = await context.Showtimes
                .Where(s => s.MovieId == movieId)
                .ToListAsync();

            return mapper.Map<IEnumerable<ShowtimeReadDto>>(showtimes);
        }
    }
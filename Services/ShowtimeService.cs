using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs.Showtime;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class ShowtimeService(AppDbContext context, IMapper mapper) : IShowtimeService
{
    public async Task<IEnumerable<ShowtimeReadDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var showtimes = await context.Showtimes
                .AsNoTracking()
                .Include(st => st.Movie)
                .Include(st => st.Hall)
                .ThenInclude(h => h.Cinema)
                .ToListAsync(cancellationToken);
            return mapper.Map<IEnumerable<ShowtimeReadDto>>(showtimes);
        }

        public async Task<ShowtimeReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var showtime = await context.Showtimes.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (showtime == null) return null;

            return mapper.Map<ShowtimeReadDto>(showtime);
        }

        public async Task<ShowtimeReadDto> CreateAsync(ShowtimeCreateDto dto, CancellationToken cancellationToken)
        {
            var showtime = mapper.Map<Showtime>(dto);
            context.Showtimes.Add(showtime);
            await context.SaveChangesAsync(cancellationToken);

            return mapper.Map<ShowtimeReadDto>(showtime);
        }

        public async Task<ShowtimeReadDto?> UpdateAsync(int id, ShowtimeUpdateDto dto, CancellationToken cancellationToken)
        {
            var showtime = await context.Showtimes.FindAsync(new object?[] { id }, cancellationToken);
            if (showtime == null) return null;

            mapper.Map(dto, showtime);
            await context.SaveChangesAsync(cancellationToken);

            return mapper.Map<ShowtimeReadDto>(showtime);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var showtime = await context.Showtimes.FindAsync(new object?[] { id }, cancellationToken);
            if (showtime == null) return false;

            context.Showtimes.Remove(showtime);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<IEnumerable<ShowtimeReadDto>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken)
        {
            var showtimes = await context.Showtimes
                .AsNoTracking()
                .Include(st => st.Movie)
                .Include(st => st.Hall)
                .ThenInclude(h => h.Cinema)
                .Where(s => s.MovieId == movieId)
                .ToListAsync(cancellationToken);


            return mapper.Map<IEnumerable<ShowtimeReadDto>>(showtimes);
        }

        public async Task<IEnumerable<ShowtimeReadDto>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken)
        {
            var showtimes = await context.Showtimes
                .AsNoTracking()
                .Include(st => st.Movie)
                .Include(st => st.Hall)
                .ThenInclude(h => h.Cinema)
                .Where(s => s.HallId == hallId)
                .ToListAsync(cancellationToken);

            return mapper.Map<IEnumerable<ShowtimeReadDto>>(showtimes);
        }
    }

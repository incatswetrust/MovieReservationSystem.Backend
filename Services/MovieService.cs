using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs.Movie;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class MovieService(AppDbContext context, IMapper mapper) : IMovieService
{
    public async Task<IEnumerable<MovieReadDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var movies = await context.Movies.AsNoTracking().ToListAsync(cancellationToken);
            return mapper.Map<IEnumerable<MovieReadDto>>(movies);
        }

        public async Task<MovieReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var movie = await context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
            if (movie == null) return null;

            return mapper.Map<MovieReadDto>(movie);
        }

        public async Task<MovieReadDto> CreateAsync(MovieCreateDto dto, CancellationToken cancellationToken)
        {
            var movie = mapper.Map<Movie>(dto);
            context.Movies.Add(movie);
            await context.SaveChangesAsync(cancellationToken);

            return mapper.Map<MovieReadDto>(movie);
        }

        public async Task<MovieReadDto?> UpdateAsync(int id, MovieUpdateDto dto, CancellationToken cancellationToken)
        {
            var movie = await context.Movies.FindAsync(new object?[] { id }, cancellationToken);
            if (movie == null) return null;

            mapper.Map(dto, movie);
            await context.SaveChangesAsync(cancellationToken);

            return mapper.Map<MovieReadDto>(movie);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var movie = await context.Movies.FindAsync(new object?[] { id }, cancellationToken);
            if (movie == null) return false;

            context.Movies.Remove(movie);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<IEnumerable<MovieReadDto>> GetByGenreAsync(string genre, CancellationToken cancellationToken)
        {
            var movies = await context.Movies
                .AsNoTracking()
                .Where(m => m.Genre != null && m.Genre == genre)
                .ToListAsync(cancellationToken);

            return mapper.Map<IEnumerable<MovieReadDto>>(movies);
        }
    }

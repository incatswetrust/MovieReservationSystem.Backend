using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs.Movie;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class MovieService(AppDbContext context, IMapper mapper) : IMovieService
{
    public async Task<IEnumerable<MovieReadDto>> GetAllAsync()
        {
            var movies = await context.Movies.ToListAsync();
            return mapper.Map<IEnumerable<MovieReadDto>>(movies);
        }

        public async Task<MovieReadDto?> GetByIdAsync(int id)
        {
            var movie = await context.Movies.FindAsync(id);
            if (movie == null) return null;

            return mapper.Map<MovieReadDto>(movie);
        }

        public async Task<MovieReadDto> CreateAsync(MovieCreateDto dto)
        {
            var movie = mapper.Map<Movie>(dto);
            context.Movies.Add(movie);
            await context.SaveChangesAsync();

            return mapper.Map<MovieReadDto>(movie);
        }

        public async Task<MovieReadDto?> UpdateAsync(int id, MovieUpdateDto dto)
        {
            var movie = await context.Movies.FindAsync(id);
            if (movie == null) return null;

            mapper.Map(dto, movie); 
            await context.SaveChangesAsync();

            return mapper.Map<MovieReadDto>(movie);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var movie = await context.Movies.FindAsync(id);
            if (movie == null) return false;

            context.Movies.Remove(movie);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<MovieReadDto>> GetByGenreAsync(string genre)
        {
            var movies = await context.Movies
                .Where(m => m.Genre != null && m.Genre == genre)
                .ToListAsync();

            return mapper.Map<IEnumerable<MovieReadDto>>(movies);
        }
    }
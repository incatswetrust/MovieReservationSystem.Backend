using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.DTOs.Movie;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class MovieService(AppDbContext context, IMapper mapper, IMemoryCache cache) : IMovieService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(45);
    private const string ListVersionKey = "movies:list-version";

    private int GetListVersion()
    {
        return cache.TryGetValue(ListVersionKey, out int version) ? version : 0;
    }

    private void BumpListVersion()
    {
        cache.Set(ListVersionKey, GetListVersion() + 1);
    }

    public async Task<IEnumerable<MovieReadDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var key = $"movies:v{GetListVersion()}:all";
            var cached = await cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                var movies = await context.Movies.AsNoTracking().ToListAsync(cancellationToken);
                return mapper.Map<IEnumerable<MovieReadDto>>(movies);
            });
            return cached ?? Enumerable.Empty<MovieReadDto>();
        }

        public async Task<PagedResult<MovieReadDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var key = $"movies:v{GetListVersion()}:page:{page}:{pageSize}";
            var cached = await cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;

                var query = context.Movies.AsNoTracking().OrderBy(m => m.Id);
                var total = await query.CountAsync(cancellationToken);
                var movies = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                return new PagedResult<MovieReadDto>
                {
                    Items = mapper.Map<List<MovieReadDto>>(movies),
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                };
            });

            return cached!;
        }

        public async Task<MovieReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var key = $"movies:{id}";
            if (cache.TryGetValue(key, out MovieReadDto? cached))
            {
                return cached;
            }

            var movie = await context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
            if (movie == null) return null;

            var dto = mapper.Map<MovieReadDto>(movie);
            cache.Set(key, dto, CacheDuration);
            return dto;
        }

        public async Task<MovieReadDto> CreateAsync(MovieCreateDto dto, CancellationToken cancellationToken)
        {
            var movie = mapper.Map<Movie>(dto);
            context.Movies.Add(movie);
            await context.SaveChangesAsync(cancellationToken);

            BumpListVersion();

            return mapper.Map<MovieReadDto>(movie);
        }

        public async Task<MovieReadDto?> UpdateAsync(int id, MovieUpdateDto dto, CancellationToken cancellationToken)
        {
            var movie = await context.Movies.FindAsync(new object?[] { id }, cancellationToken);
            if (movie == null) return null;

            mapper.Map(dto, movie);
            await context.SaveChangesAsync(cancellationToken);

            cache.Remove($"movies:{id}");
            BumpListVersion();

            return mapper.Map<MovieReadDto>(movie);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var movie = await context.Movies.FindAsync(new object?[] { id }, cancellationToken);
            if (movie == null) return false;

            context.Movies.Remove(movie);
            await context.SaveChangesAsync(cancellationToken);

            cache.Remove($"movies:{id}");
            BumpListVersion();

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

        public async Task<IEnumerable<MovieReadDto>> SearchAsync(string q, CancellationToken cancellationToken)
        {
            var movies = await context.Movies
                .AsNoTracking()
                .Where(m => EF.Functions.Like(m.Title, $"%{q}%") ||
                            (m.Director != null && EF.Functions.Like(m.Director, $"%{q}%")))
                .ToListAsync(cancellationToken);

            return mapper.Map<IEnumerable<MovieReadDto>>(movies);
        }

        public async Task<IEnumerable<MovieReadDto>> FilterAsync(string? genre, int? year, string? rating, CancellationToken cancellationToken)
        {
            var query = context.Movies.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(genre))
                query = query.Where(m => m.Genre != null && m.Genre == genre);

            if (year.HasValue)
                query = query.Where(m => m.ReleaseYear == year.Value);

            if (!string.IsNullOrWhiteSpace(rating))
                query = query.Where(m => m.AgeRating != null && m.AgeRating == rating);

            var movies = await query.ToListAsync(cancellationToken);
            return mapper.Map<IEnumerable<MovieReadDto>>(movies);
        }
    }

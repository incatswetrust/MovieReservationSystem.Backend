using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.DTOs.Showtime;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class ShowtimeService(AppDbContext context, IMapper mapper, IMemoryCache cache) : IShowtimeService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(45);
    private static CancellationTokenSource _listCacheResetSource = new();

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

        public async Task<PagedResult<ShowtimeReadDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var cacheKey = $"showtimes:page:{page}:{pageSize}";
            var cached = await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SetAbsoluteExpiration(CacheDuration);
                entry.AddExpirationToken(new CancellationChangeToken(_listCacheResetSource.Token));

                var query = context.Showtimes
                    .AsNoTracking()
                    .Include(st => st.Movie)
                    .Include(st => st.Hall)
                    .ThenInclude(h => h.Cinema)
                    .OrderBy(st => st.Id);

                var total = await query.CountAsync(cancellationToken);
                var showtimes = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                return new PagedResult<ShowtimeReadDto>
                {
                    Items = mapper.Map<List<ShowtimeReadDto>>(showtimes),
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                };
            });

            return cached!;
        }

        public async Task<ShowtimeReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var cacheKey = $"showtimes:{id}";
            if (cache.TryGetValue(cacheKey, out ShowtimeReadDto? cached))
            {
                return cached;
            }

            var showtime = await context.Showtimes
                .AsNoTracking()
                .Include(st => st.Movie)
                .Include(st => st.Hall)
                .ThenInclude(h => h.Cinema)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (showtime == null) return null;

            var dto = mapper.Map<ShowtimeReadDto>(showtime);
            cache.Set(cacheKey, dto, CacheDuration);
            return dto;
        }

        public async Task<ShowtimeReadDto> CreateAsync(ShowtimeCreateDto dto, CancellationToken cancellationToken)
        {
            var showtime = mapper.Map<Showtime>(dto);
            context.Showtimes.Add(showtime);
            await context.SaveChangesAsync(cancellationToken);

            InvalidateListCache();

            return mapper.Map<ShowtimeReadDto>(showtime);
        }

        public async Task<ShowtimeReadDto?> UpdateAsync(int id, ShowtimeUpdateDto dto, CancellationToken cancellationToken)
        {
            var showtime = await context.Showtimes.FindAsync(new object?[] { id }, cancellationToken);
            if (showtime == null) return null;

            mapper.Map(dto, showtime);
            await context.SaveChangesAsync(cancellationToken);

            InvalidateListCache();
            cache.Remove($"showtimes:{id}");

            return mapper.Map<ShowtimeReadDto>(showtime);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var showtime = await context.Showtimes.FindAsync(new object?[] { id }, cancellationToken);
            if (showtime == null) return false;

            context.Showtimes.Remove(showtime);
            await context.SaveChangesAsync(cancellationToken);

            InvalidateListCache();
            cache.Remove($"showtimes:{id}");

            return true;
        }

        // Cancels the shared expiration token so all cached "showtimes:page:*" entries
        // are invalidated together, since individual page keys aren't tracked.
        private static void InvalidateListCache()
        {
            var previous = Interlocked.Exchange(ref _listCacheResetSource, new CancellationTokenSource());
            if (!previous.IsCancellationRequested)
            {
                previous.Cancel();
            }
            previous.Dispose();
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

        public async Task<IEnumerable<ShowtimeReadDto>> GetAvailableAsync(DateOnly date, CancellationToken cancellationToken)
        {
            var rangeStart = date.ToDateTime(TimeOnly.MinValue);
            var rangeEnd = rangeStart.AddDays(1);

            var showtimes = await context.Showtimes
                .AsNoTracking()
                .Include(st => st.Movie)
                .Include(st => st.Hall)
                .ThenInclude(h => h.Cinema)
                .Where(st => st.StartTime >= rangeStart && st.StartTime < rangeEnd)
                .OrderBy(st => st.StartTime)
                .ToListAsync(cancellationToken);

            return mapper.Map<IEnumerable<ShowtimeReadDto>>(showtimes);
        }
    }

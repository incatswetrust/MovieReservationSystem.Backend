using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.DTOs.Cinema;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class CinemaService(AppDbContext context, IMapper mapper, IMemoryCache cache) : ICinemaService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(45);
    private const string ListVersionKey = "cinemas:list-version";

    private int GetListVersion()
    {
        return cache.TryGetValue(ListVersionKey, out int version) ? version : 0;
    }

    private void BumpListVersion()
    {
        cache.Set(ListVersionKey, GetListVersion() + 1);
    }

    public async Task<IEnumerable<CinemaReadDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var key = $"cinemas:v{GetListVersion()}:all";
        var cached = await cache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            var cinemas = await context.Cinemas.AsNoTracking().ToListAsync(cancellationToken);
            return mapper.Map<IEnumerable<CinemaReadDto>>(cinemas);
        });
        return cached ?? Enumerable.Empty<CinemaReadDto>();
    }

    public async Task<PagedResult<CinemaReadDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : pageSize;

        var key = $"cinemas:v{GetListVersion()}:page:{page}:{pageSize}";
        var cached = await cache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;

            var query = context.Cinemas.AsNoTracking().OrderBy(c => c.Id);
            var total = await query.CountAsync(cancellationToken);
            var cinemas = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<CinemaReadDto>
            {
                Items = mapper.Map<List<CinemaReadDto>>(cinemas),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        });

        return cached!;
    }

    public async Task<CinemaReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var key = $"cinemas:{id}";
        if (cache.TryGetValue(key, out CinemaReadDto? cached))
        {
            return cached;
        }

        var cinema = await context.Cinemas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (cinema == null) return null;

        var dto = mapper.Map<CinemaReadDto>(cinema);
        cache.Set(key, dto, CacheDuration);
        return dto;
    }

    public async Task<CinemaReadDto> CreateAsync(CinemaCreateDto dto, CancellationToken cancellationToken)
    {
        var cinema = mapper.Map<Cinema>(dto);
        context.Cinemas.Add(cinema);
        await context.SaveChangesAsync(cancellationToken);

        BumpListVersion();

        return mapper.Map<CinemaReadDto>(cinema);
    }

    public async Task<CinemaReadDto?> UpdateAsync(int id, CinemaUpdateDto dto, CancellationToken cancellationToken)
    {
        var cinema = await context.Cinemas.FindAsync(new object?[] { id }, cancellationToken);
        if (cinema == null) return null;

        mapper.Map(dto, cinema);
        await context.SaveChangesAsync(cancellationToken);

        cache.Remove($"cinemas:{id}");
        BumpListVersion();

        return mapper.Map<CinemaReadDto>(cinema);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var cinema = await context.Cinemas.FindAsync(new object?[] { id }, cancellationToken);
        if (cinema == null) return false;

        context.Cinemas.Remove(cinema);
        await context.SaveChangesAsync(cancellationToken);

        cache.Remove($"cinemas:{id}");
        BumpListVersion();

        return true;
    }

    public async Task<IEnumerable<CinemaReadDto>> SearchAsync(string q, CancellationToken cancellationToken)
    {
        var cinemas = await context.Cinemas
            .AsNoTracking()
            .Where(c => EF.Functions.Like(c.Name, $"%{q}%") ||
                        (c.Address != null && EF.Functions.Like(c.Address, $"%{q}%")))
            .ToListAsync(cancellationToken);

        return mapper.Map<IEnumerable<CinemaReadDto>>(cinemas);
    }
}

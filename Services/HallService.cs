using System.Collections;
using System.Collections.Concurrent;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.DTOs.Hall;
using MovieReservationSystem.Backend.DTOs.Seat;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class HallService(AppDbContext context, IMapper mapper, IMemoryCache cache) : IHallService
{
    // Tracks every "halls:page:{page}:{pageSize}" key we've cached so we can evict them all
    // on mutation. IMemoryCache has no native way to enumerate/prefix-remove keys.
    private static readonly ConcurrentDictionary<string, byte> _listCacheKeys = new();
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(45);

    private static string ListCacheKey(int page, int pageSize) => $"halls:page:{page}:{pageSize}";
    private static string IdCacheKey(int id) => $"halls:{id}";

    private void InvalidateListCache()
    {
        foreach (var key in _listCacheKeys.Keys)
        {
            cache.Remove(key);
        }
        _listCacheKeys.Clear();
    }

    public void InvalidateHallCache(int id)
    {
        cache.Remove(IdCacheKey(id));
    }

    public async Task<IEnumerable<HallReadDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var halls = await context.Halls
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return mapper.Map<IEnumerable<HallReadDto>>(halls);
    }

    public async Task<PagedResult<HallReadDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : pageSize;

        var cacheKey = ListCacheKey(page, pageSize);
        if (cache.TryGetValue(cacheKey, out PagedResult<HallReadDto>? cached) && cached != null)
        {
            return cached;
        }

        var query = context.Halls.AsNoTracking().OrderBy(h => h.Id);
        var total = await query.CountAsync(cancellationToken);
        var halls = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var result = new PagedResult<HallReadDto>
        {
            Items = mapper.Map<List<HallReadDto>>(halls),
            Total = total,
            Page = page,
            PageSize = pageSize
        };

        cache.Set(cacheKey, result, CacheDuration);
        _listCacheKeys.TryAdd(cacheKey, 0);

        return result;
    }

    public async Task<HallReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var cacheKey = IdCacheKey(id);
        if (cache.TryGetValue(cacheKey, out HallReadDto? cached) && cached != null)
        {
            return cached;
        }

        var hall = await context.Halls.AsNoTracking().Include(h => h.Seats).Include(h => h.Images)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
        if (hall == null) return null;

        var dto = mapper.Map<HallReadDto>(hall);
        cache.Set(cacheKey, dto, CacheDuration);
        return dto;
    }

    public async Task<HallReadDto> CreateAsync(HallCreateDto dto, CancellationToken cancellationToken)
    {
        var hall = mapper.Map<Hall>(dto);
        context.Halls.Add(hall);
        await context.SaveChangesAsync(cancellationToken);
        var seatsToAdd = new List<Seat>();

        if (dto.NumberOfRows > 0 && dto.SeatsPerRow > 0)
        {
            for (int row = 0; row < dto.NumberOfRows; row++)
            {
                char rowLabelChar = (char)('A' + row);
                var rowLabel = rowLabelChar.ToString();

                for (int seatNum = 1; seatNum <= dto.SeatsPerRow; seatNum++)
                {
                    seatsToAdd.Add(new Seat
                    {
                        HallId = hall.Id,
                        RowLabel = rowLabel,
                        SeatNumber = seatNum
                    });
                }
            }
        }
        if (seatsToAdd.Count > 0)
        {
            context.Seats.AddRange(seatsToAdd);
            await context.SaveChangesAsync(cancellationToken);
        }
        await context.Entry(hall).Collection(h => h.Seats).LoadAsync(cancellationToken);
        var readDto = mapper.Map<HallReadDto>(hall);
        InvalidateListCache();
        return readDto;
    }

    public async Task<HallReadDto?> UpdateAsync(int id, HallUpdateDto dto, CancellationToken cancellationToken)
    {
        var hall = await context.Halls.FindAsync(new object?[] { id }, cancellationToken);
        if (hall == null) return null;

        mapper.Map(dto, hall);
        await context.SaveChangesAsync(cancellationToken);

        InvalidateListCache();
        InvalidateHallCache(id);
        return mapper.Map<HallReadDto>(hall);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var hall = await context.Halls.FindAsync(new object?[] { id }, cancellationToken);
        if (hall == null) return false;

        context.Halls.Remove(hall);
        await context.SaveChangesAsync(cancellationToken);

        InvalidateListCache();
        InvalidateHallCache(id);
        return true;
    }

    public async Task<IEnumerable<HallReadDto>> GetByCinemaId(int cinemaId, CancellationToken cancellationToken)
    {
        var halls = await context.Halls
            .AsNoTracking()
            .Where(hall => hall.CinemaId == cinemaId)
            .Include(h => h.Seats)
            .Include(h => h.Images)
            .ToListAsync(cancellationToken);

        return mapper.Map<IEnumerable<HallReadDto>>(halls);
    }

    public async Task<IEnumerable<HallReadDto>> SearchAsync(string q, CancellationToken cancellationToken)
    {
        var halls = await context.Halls
            .AsNoTracking()
            .Where(h => EF.Functions.Like(h.Name, $"%{q}%"))
            .ToListAsync(cancellationToken);

        return mapper.Map<IEnumerable<HallReadDto>>(halls);
    }
}

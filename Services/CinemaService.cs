using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.DTOs.Cinema;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class CinemaService(AppDbContext context, IMapper mapper) : ICinemaService
{
    public async Task<IEnumerable<CinemaReadDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var cinemas = await context.Cinemas.AsNoTracking().ToListAsync(cancellationToken);
        return mapper.Map<IEnumerable<CinemaReadDto>>(cinemas);
    }

    public async Task<PagedResult<CinemaReadDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : pageSize;

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
    }

    public async Task<CinemaReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var cinema = await context.Cinemas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (cinema == null) return null;

        return mapper.Map<CinemaReadDto>(cinema);
    }

    public async Task<CinemaReadDto> CreateAsync(CinemaCreateDto dto, CancellationToken cancellationToken)
    {
        var cinema = mapper.Map<Cinema>(dto);
        context.Cinemas.Add(cinema);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<CinemaReadDto>(cinema);
    }

    public async Task<CinemaReadDto?> UpdateAsync(int id, CinemaUpdateDto dto, CancellationToken cancellationToken)
    {
        var cinema = await context.Cinemas.FindAsync(new object?[] { id }, cancellationToken);
        if (cinema == null) return null;

        mapper.Map(dto, cinema);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<CinemaReadDto>(cinema);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var cinema = await context.Cinemas.FindAsync(new object?[] { id }, cancellationToken);
        if (cinema == null) return false;

        context.Cinemas.Remove(cinema);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

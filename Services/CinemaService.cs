using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
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

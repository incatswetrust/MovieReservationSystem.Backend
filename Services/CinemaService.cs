using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs.Cinema;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class CinemaService(AppDbContext context, IMapper mapper) : ICinemaService
{
    public async Task<IEnumerable<CinemaReadDto>> GetAllAsync()
    {
        var cinemas = await context.Cinemas.ToListAsync();
        return mapper.Map<IEnumerable<CinemaReadDto>>(cinemas);
    }

    public async Task<CinemaReadDto?> GetByIdAsync(int id)
    {
        var cinema = await context.Cinemas.FindAsync(id);
        if (cinema == null) return null;

        return mapper.Map<CinemaReadDto>(cinema);
    }

    public async Task<CinemaReadDto> CreateAsync(CinemaCreateDto dto)
    {
        var cinema = mapper.Map<Cinema>(dto);
        context.Cinemas.Add(cinema);
        await context.SaveChangesAsync();

        return mapper.Map<CinemaReadDto>(cinema);
    }

    public async Task<CinemaReadDto?> UpdateAsync(int id, CinemaUpdateDto dto)
    {
        var cinema = await context.Cinemas.FindAsync(id);
        if (cinema == null) return null;

        mapper.Map(dto, cinema);
        await context.SaveChangesAsync();

        return mapper.Map<CinemaReadDto>(cinema);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var cinema = await context.Cinemas.FindAsync(id);
        if (cinema == null) return false;

        context.Cinemas.Remove(cinema);
        await context.SaveChangesAsync();
        return true;
    }
}
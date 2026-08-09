using System.Collections;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs.Hall;
using MovieReservationSystem.Backend.DTOs.Seat;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class HallService(AppDbContext context, IMapper mapper) : IHallService
{
    public async Task<IEnumerable<HallReadDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var halls = await context.Halls
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return mapper.Map<IEnumerable<HallReadDto>>(halls);
    }

    public async Task<HallReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var hall = await context.Halls.AsNoTracking().Include(h => h.Seats).Include(h => h.Images)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
        return hall == null ? null : mapper.Map<HallReadDto>(hall);
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
        return readDto;
    }

    public async Task<HallReadDto?> UpdateAsync(int id, HallUpdateDto dto, CancellationToken cancellationToken)
    {
        var hall = await context.Halls.FindAsync(new object?[] { id }, cancellationToken);
        if (hall == null) return null;

        mapper.Map(dto, hall);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<HallReadDto>(hall);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var hall = await context.Halls.FindAsync(new object?[] { id }, cancellationToken);
        if (hall == null) return false;

        context.Halls.Remove(hall);
        await context.SaveChangesAsync(cancellationToken);
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
}

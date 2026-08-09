using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.DTOs.Seat;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class SeatService(AppDbContext context, IMapper mapper) : ISeatService
{
    public async Task<IEnumerable<SeatReadDto>> GetSeatsByShowtimeAsync(int showtimeId, CancellationToken cancellationToken)
    {
        var showtime = await context.Showtimes
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == showtimeId, cancellationToken);

        if (showtime == null)
        {
            return Enumerable.Empty<SeatReadDto>();
        }
        var seats = await context.Seats
            .AsNoTracking()
            .Where(s => s.HallId == showtime.HallId)
            .ToListAsync(cancellationToken);

        var bookedSeatIds = await (
            from bs in context.BookedSeats
            join b in context.Bookings on bs.BookingId equals b.Id
            where b.ShowtimeId == showtimeId && b.Status != "Canceled"
            select bs.SeatId
        ).ToListAsync(cancellationToken);
        var seatDtos = mapper.Map<List<SeatReadDto>>(seats);
        var bookedSet = new HashSet<int>(bookedSeatIds);
        foreach (var seatDto in seatDtos)
        {
            seatDto.IsReserved = bookedSet.Contains(seatDto.Id);
        }

        return seatDtos;
    }
}

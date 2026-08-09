using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.DTOs.Booking;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class BookingService(AppDbContext context, IMapper mapper) : IBookingService
{
    public async Task<IEnumerable<BookingReadDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var bookings = await context.Bookings
                .AsNoTracking()
                .Include(b => b.BookedSeats)
                .ToListAsync(cancellationToken);

            return MapWithSeatIds(bookings);
        }

        public async Task<PagedResult<BookingReadDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var query = context.Bookings.AsNoTracking().OrderBy(b => b.Id);
            var total = await query.CountAsync(cancellationToken);
            var bookings = await query
                .Include(b => b.BookedSeats)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<BookingReadDto>
            {
                Items = MapWithSeatIds(bookings).ToList(),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        private IEnumerable<BookingReadDto> MapWithSeatIds(IEnumerable<Booking> bookings)
        {
            foreach (var booking in bookings)
            {
                var dto = mapper.Map<BookingReadDto>(booking);
                if (booking.BookedSeats != null)
                    dto.SeatIds = booking.BookedSeats.Select(bs => bs.SeatId).ToList();
                yield return dto;
            }
        }

        public async Task<BookingReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var booking = await context.Bookings
                .AsNoTracking()
                .Include(b => b.BookedSeats)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

            if (booking == null) return null;
            var bookingDto = mapper.Map<BookingReadDto>(booking);
            if (booking.BookedSeats != null)
            {
                bookingDto.SeatIds = booking.BookedSeats
                    .Select(bs => bs.SeatId)
                    .ToList();
            }

            return bookingDto;
        }

        public async Task<BookingReadDto> CreateAsync(BookingCreateDto dto, CancellationToken cancellationToken)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var alreadyBookedSeats = await (
                    from seat in context.BookedSeats
                    join booking in context.Bookings
                        on seat.BookingId equals booking.Id
                    where dto.SeatIds.Contains(seat.SeatId)
                          && booking.ShowtimeId == dto.ShowtimeId
                          && booking.Status != "Canceled"
                    select seat.SeatId
                ).ToListAsync(cancellationToken);

                if (alreadyBookedSeats.Any())
                {
                    throw new Exception("Some seats are already booked: "
                                        + string.Join(", ", alreadyBookedSeats));
                }
                var boking = new Booking
                {
                    UserId = dto.UserId,
                    ShowtimeId = dto.ShowtimeId,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    TotalPrice = 0m
                };

                context.Bookings.Add(boking);
                await context.SaveChangesAsync(cancellationToken);
                var bookedSeatsToAdd = new List<BookedSeat>();
                foreach (var seatId in dto.SeatIds)
                {
                    bookedSeatsToAdd.Add(new BookedSeat
                    {
                        BookingId = boking.Id,
                        SeatId = seatId,
                        Price = 0
                    });
                }

                context.BookedSeats.AddRange(bookedSeatsToAdd);
                await context.SaveChangesAsync(cancellationToken);
                var showtime = await context.Showtimes.FindAsync(new object?[] { dto.ShowtimeId }, cancellationToken);
                if (showtime != null)
                {
                    boking.TotalPrice = showtime.Price * dto.SeatIds.Count;
                    await context.SaveChangesAsync(cancellationToken);
                }
                await transaction.CommitAsync(cancellationToken);
                await context.Entry(boking).Collection(b => b.BookedSeats!).LoadAsync(cancellationToken);

                var readDto = mapper.Map<BookingReadDto>(boking);
                if (boking.BookedSeats != null) readDto.SeatIds = boking.BookedSeats.Select(bs => bs.SeatId).ToList();

                return readDto;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<BookingReadDto?> UpdateAsync(int id, BookingUpdateDto dto, CancellationToken cancellationToken)
        {
            var booking = await context.Bookings
                .Include(b => b.BookedSeats)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

            if (booking == null) return null;
            booking.Status = dto.Status;
            await context.SaveChangesAsync(cancellationToken);

            var readDto = mapper.Map<BookingReadDto>(booking);
            if (booking.BookedSeats != null)
            {
                readDto.SeatIds = booking.BookedSeats.Select(bs => bs.SeatId).ToList();
            }
            return readDto;
        }
        public async Task<bool> CancelAsync(int id, CancellationToken cancellationToken)
        {
            var booking = await context.Bookings.FindAsync(new object?[] { id }, cancellationToken);
            if (booking == null) return false;
            booking.Status = "Canceled";
            await context.SaveChangesAsync(cancellationToken);

            // _context.Bookings.Remove(booking);
            // await _context.SaveChangesAsync();

            return true;
        }
    }

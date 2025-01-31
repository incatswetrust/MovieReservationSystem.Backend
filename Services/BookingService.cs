using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs.Booking;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class BookingService(AppDbContext context, IMapper mapper) : IBookingService
{
    public async Task<IEnumerable<BookingReadDto>> GetAllAsync()
        {
            var bookings = await context.Bookings
                .ToListAsync();

            var result = mapper.Map<IEnumerable<BookingReadDto>>(bookings);
            return result;
        }

        public async Task<BookingReadDto?> GetByIdAsync(int id)
        {
            var booking = await context.Bookings
                .Include(b => b.BookedSeats)
                .FirstOrDefaultAsync(b => b.Id == id);

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

        public async Task<BookingReadDto> CreateAsync(BookingCreateDto dto)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

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
                ).ToListAsync();

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
                await context.SaveChangesAsync();
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
                await context.SaveChangesAsync();
                var showtime = await context.Showtimes.FindAsync(dto.ShowtimeId);
                if (showtime != null)
                {
                    boking.TotalPrice = showtime.Price * dto.SeatIds.Count;
                    await context.SaveChangesAsync();
                }
                await transaction.CommitAsync();
                await context.Entry(boking).Collection(b => b.BookedSeats!).LoadAsync();

                var readDto = mapper.Map<BookingReadDto>(boking);
                if (boking.BookedSeats != null) readDto.SeatIds = boking.BookedSeats.Select(bs => bs.SeatId).ToList();

                return readDto;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<BookingReadDto?> UpdateAsync(int id, BookingUpdateDto dto)
        {
            var booking = await context.Bookings
                .Include(b => b.BookedSeats)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return null;
            booking.Status = dto.Status;
            await context.SaveChangesAsync();

            var readDto = mapper.Map<BookingReadDto>(booking);
            if (booking.BookedSeats != null)
            {
                readDto.SeatIds = booking.BookedSeats.Select(bs => bs.SeatId).ToList();
            }
            return readDto;
        }
        public async Task<bool> CancelAsync(int id)
        {
            var booking = await context.Bookings.FindAsync(id);
            if (booking == null) return false;
            booking.Status = "Canceled";
            await context.SaveChangesAsync();
            
            // _context.Bookings.Remove(booking);
            // await _context.SaveChangesAsync();

            return true;
        }
    }
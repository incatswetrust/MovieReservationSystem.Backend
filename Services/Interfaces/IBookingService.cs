using MovieReservationSystem.Backend.DTOs.Booking;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<BookingReadDto>> GetAllAsync();
    Task<BookingReadDto?> GetByIdAsync(int id);
    Task<BookingReadDto> CreateAsync(BookingCreateDto dto);
    Task<BookingReadDto?> UpdateAsync(int id, BookingUpdateDto dto);
    Task<bool> CancelAsync(int id);
}
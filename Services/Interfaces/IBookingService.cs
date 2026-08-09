using MovieReservationSystem.Backend.DTOs.Booking;

namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<BookingReadDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<BookingReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<BookingReadDto> CreateAsync(BookingCreateDto dto, CancellationToken cancellationToken);
    Task<BookingReadDto?> UpdateAsync(int id, BookingUpdateDto dto, CancellationToken cancellationToken);
    Task<bool> CancelAsync(int id, CancellationToken cancellationToken);
}

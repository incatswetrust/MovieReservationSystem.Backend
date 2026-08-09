using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.DTOs.Booking;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<PagedResult<BookingReadDto>>> GetAll(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var allBookings = await bookingService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(allBookings);
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<BookingReadDto>>> GetMyBookings(CancellationToken cancellationToken)
    {
        var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ErrorResponse("User is not authenticated"));
        var userId = int.Parse(userIdStr);
        var all = await bookingService.GetAllAsync(cancellationToken);
        var myBookings = all.Where(b => b.UserId == userId);

        return Ok(myBookings);
    }
    [Authorize(Roles = "User,Admin")]
    [HttpPost]
    public async Task<ActionResult<BookingReadDto>> Create(BookingCreateDto dto, CancellationToken cancellationToken)
    {
        var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ErrorResponse("User is not authenticated"));

        dto.UserId = int.Parse(userIdStr);

        var createdBooking = await bookingService.CreateAsync(dto, cancellationToken);
        return Ok(createdBooking);
    }
    [Authorize(Roles = "User,Admin")]
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var booking = await bookingService.GetByIdAsync(id, cancellationToken);
        if (booking == null) return NotFound(new ErrorResponse("Booking not found"));
        var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var currentRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        if (currentRole != "Admin" && booking.UserId.ToString() != currentUserId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse("You can only cancel your own bookings."));
        }

        var success = await bookingService.CancelAsync(id, cancellationToken);
        if (!success) return BadRequest(new ErrorResponse("Could not cancel booking."));
        return Ok(new { message = "Booking canceled." });
    }
}

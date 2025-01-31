using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs.Booking;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var allBookings = await bookingService.GetAllAsync();
        return Ok(allBookings);
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
        var userId = int.Parse(userIdStr);
        var all = await bookingService.GetAllAsync();
        var myBookings = all.Where(b => b.UserId == userId);

        return Ok(myBookings);
    }
    [Authorize(Roles = "User,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(BookingCreateDto dto)
    {
        var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        dto.UserId = int.Parse(userIdStr);

        var createdBooking = await bookingService.CreateAsync(dto);
        return Ok(createdBooking);
    }
    [Authorize(Roles = "User,Admin")]
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var booking = await bookingService.GetByIdAsync(id);
        if (booking == null) return NotFound("Booking not found");
        var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var currentRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        if (currentRole != "Admin" && booking.UserId.ToString() != currentUserId)
        {
            return Forbid("You can only cancel your own bookings.");
        }

        var success = await bookingService.CancelAsync(id);
        if (!success) return BadRequest("Could not cancel booking.");
        return Ok("Booking canceled.");
    }
}
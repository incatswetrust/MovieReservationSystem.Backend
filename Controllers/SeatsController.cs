using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs.Seat;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;


[ApiController]
[Route("api/[controller]")]
public class SeatsController(ISeatService seatService): ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<SeatReadDto>>> GetAll(int id, CancellationToken cancellationToken)
    {
        var seats = await seatService.GetSeatsByShowtimeAsync(id, cancellationToken);
        return Ok(seats);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs.Movie;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;


[ApiController]
[Route("api/[controller]")]
public class SeatsController(ISeatService seatService): ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<MovieReadDto>>> GetAll(int id)
    {
        var seats = await seatService.GetSeatsByShowtimeAsync(id);
        return Ok(seats);
    }
}
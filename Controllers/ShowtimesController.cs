using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs.Showtime;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowtimesController(IShowtimeService showtimeService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShowtimeReadDto>>> GetAll()
    {
        var showtimes = await showtimeService.GetAllAsync();
        return Ok(showtimes);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<ShowtimeReadDto>> GetById(int id)
    {
        var showtime = await showtimeService.GetByIdAsync(id);
        if (showtime == null) return NotFound();
        return Ok(showtime);
    }

    [AllowAnonymous]
    [HttpGet("movie/{movieId}")]
    public async Task<ActionResult<IEnumerable<ShowtimeReadDto>>> GetByMovieId(int movieId)
    {
        var showtimes = await showtimeService.GetByMovieIdAsync(movieId);
        return Ok(showtimes);
    }
    
    [AllowAnonymous]
    [HttpGet("hall/{movieId}")]
    public async Task<ActionResult<IEnumerable<ShowtimeReadDto>>> GetByHallId(int movieId)
    {
        var showtimes = await showtimeService.GetByHallIdAsync(movieId);
        return Ok(showtimes);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ShowtimeReadDto>> Create(ShowtimeCreateDto dto)
    {
        var created = await showtimeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ShowtimeReadDto>> Update(int id, ShowtimeUpdateDto dto)
    {
        var updated = await showtimeService.UpdateAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await showtimeService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
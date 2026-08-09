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
    public async Task<ActionResult<IEnumerable<ShowtimeReadDto>>> GetAll(CancellationToken cancellationToken)
    {
        var showtimes = await showtimeService.GetAllAsync(cancellationToken);
        return Ok(showtimes);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<ShowtimeReadDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var showtime = await showtimeService.GetByIdAsync(id, cancellationToken);
        if (showtime == null) return NotFound();
        return Ok(showtime);
    }

    [AllowAnonymous]
    [HttpGet("movie/{movieId}")]
    public async Task<ActionResult<IEnumerable<ShowtimeReadDto>>> GetByMovieId(int movieId, CancellationToken cancellationToken)
    {
        var showtimes = await showtimeService.GetByMovieIdAsync(movieId, cancellationToken);
        return Ok(showtimes);
    }

    [AllowAnonymous]
    [HttpGet("hall/{hallId}")]
    public async Task<ActionResult<IEnumerable<ShowtimeReadDto>>> GetByHallId(int hallId, CancellationToken cancellationToken)
    {
        var showtimes = await showtimeService.GetByHallIdAsync(hallId, cancellationToken);
        return Ok(showtimes);
    }

    [AllowAnonymous]
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<ShowtimeReadDto>>> GetAvailable([FromQuery] DateOnly date, CancellationToken cancellationToken)
    {
        var showtimes = await showtimeService.GetAvailableAsync(date, cancellationToken);
        return Ok(showtimes);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ShowtimeReadDto>> Create(ShowtimeCreateDto dto, CancellationToken cancellationToken)
    {
        var created = await showtimeService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ShowtimeReadDto>> Update(int id, ShowtimeUpdateDto dto, CancellationToken cancellationToken)
    {
        var updated = await showtimeService.UpdateAsync(id, dto, cancellationToken);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var success = await showtimeService.DeleteAsync(id, cancellationToken);
        if (!success) return NotFound();
        return NoContent();
    }
}

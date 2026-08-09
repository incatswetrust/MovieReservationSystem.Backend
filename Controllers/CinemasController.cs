using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.DTOs.Cinema;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CinemasController(ICinemaService cinemaService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<CinemaReadDto>>> GetAll(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var cinemas = await cinemaService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(cinemas);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<CinemaReadDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var cinema = await cinemaService.GetByIdAsync(id, cancellationToken);
        if (cinema == null) return NotFound();
        return Ok(cinema);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CinemaReadDto>> Create(CinemaCreateDto dto, CancellationToken cancellationToken)
    {
        var created = await cinemaService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<CinemaReadDto>> Update(int id, CinemaUpdateDto dto, CancellationToken cancellationToken)
    {
        var updated = await cinemaService.UpdateAsync(id, dto, cancellationToken);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var success = await cinemaService.DeleteAsync(id, cancellationToken);
        if (!success) return NotFound();
        return NoContent();
    }
}

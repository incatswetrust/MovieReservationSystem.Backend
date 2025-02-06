using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs.Cinema;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CinemasController(ICinemaService cinemaService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CinemaReadDto>>> GetAll()
    {
        var cinemas = await cinemaService.GetAllAsync();
        return Ok(cinemas);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<CinemaReadDto>> GetById(int id)
    {
        var cinema = await cinemaService.GetByIdAsync(id);
        if (cinema == null) return NotFound();
        return Ok(cinema);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CinemaReadDto>> Create(CinemaCreateDto dto)
    {
        var created = await cinemaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<CinemaReadDto>> Update(int id, CinemaUpdateDto dto)
    {
        var updated = await cinemaService.UpdateAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await cinemaService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
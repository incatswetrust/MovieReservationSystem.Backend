using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs.Cinema;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CinemasController : ControllerBase
{
    private readonly ICinemaService _cinemaService;

    public CinemasController(ICinemaService cinemaService)
    {
        _cinemaService = cinemaService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var cinemas = await _cinemaService.GetAllAsync();
        return Ok(cinemas);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var cinema = await _cinemaService.GetByIdAsync(id);
        if (cinema == null) return NotFound();
        return Ok(cinema);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CinemaCreateDto dto)
    {
        var created = await _cinemaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CinemaUpdateDto dto)
    {
        var updated = await _cinemaService.UpdateAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _cinemaService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
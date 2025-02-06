using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs.Hall;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HallsController(IHallService hallService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<HallReadDto>>> GetAll()
    {
        var halls = await hallService.GetAllAsync();
        return Ok(halls);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<HallReadDto>> GetById(int id)
    {
        var hall = await hallService.GetByIdAsync(id);
        if (hall == null) return NotFound();
        return Ok(hall);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<HallReadDto>> Create(HallCreateDto dto)
    {
        var created = await hallService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<HallReadDto>> Update(int id, HallUpdateDto dto)
    {
        var updated = await hallService.UpdateAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await hallService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
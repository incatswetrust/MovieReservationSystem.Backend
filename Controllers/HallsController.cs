using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.DTOs.Hall;
using MovieReservationSystem.Backend.DTOs.HallImage;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HallsController(IHallService hallService, IHallImageService hallImageService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<HallReadDto>>> GetAll(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var halls = await hallService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(halls);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<HallReadDto>>> Search([FromQuery] string q, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(q)) return Ok(Enumerable.Empty<HallReadDto>());
        var halls = await hallService.SearchAsync(q, cancellationToken);
        return Ok(halls);
    }

    [HttpGet("byCinema/{cinemaId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<HallReadDto>>> GetByCinemaId(int cinemaId, CancellationToken cancellationToken)
    {
        var halls = await hallService.GetByCinemaId(cinemaId, cancellationToken);
        if (halls == null) return NotFound();
        return Ok(halls);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<HallReadDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var hall = await hallService.GetByIdAsync(id, cancellationToken);
        if (hall == null) return NotFound();
        return Ok(hall);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<HallReadDto>> Create(HallCreateDto dto, CancellationToken cancellationToken)
    {
        var created = await hallService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<HallReadDto>> Update(int id, HallUpdateDto dto, CancellationToken cancellationToken)
    {
        var updated = await hallService.UpdateAsync(id, dto, cancellationToken);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var success = await hallService.DeleteAsync(id, cancellationToken);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpGet("{id}/images")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<HallImageReadDto>>> GetImages(int id, CancellationToken cancellationToken)
    {
        var images = await hallImageService.GetByHallIdAsync(id, cancellationToken);
        return Ok(images);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/images")]
    public async Task<ActionResult<HallImageReadDto>> AddImage(int id, HallImageCreateDto dto, CancellationToken cancellationToken)
    {
        var created = await hallImageService.CreateAsync(id, dto, cancellationToken);
        hallService.InvalidateHallCache(id);
        return CreatedAtAction(nameof(GetImages), new { id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}/images/{imageId}")]
    public async Task<ActionResult> DeleteImage(int id, int imageId, CancellationToken cancellationToken)
    {
        var success = await hallImageService.DeleteAsync(id, imageId, cancellationToken);
        if (!success) return NotFound();
        hallService.InvalidateHallCache(id);
        return NoContent();
    }
}

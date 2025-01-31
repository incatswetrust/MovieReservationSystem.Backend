using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs.Movie;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var movies = await movieService.GetAllAsync();
        return Ok(movies);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var movie = await movieService.GetByIdAsync(id);
        if (movie == null) return NotFound();
        return Ok(movie);
    }

    [AllowAnonymous]
    [HttpGet("genre/{genre}")]
    public async Task<IActionResult> GetByGenre(string genre)
    {
        var movies = await movieService.GetByGenreAsync(genre);
        return Ok(movies);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(MovieCreateDto dto)
    {
        var created = await movieService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, MovieUpdateDto dto)
    {
        var updated = await movieService.UpdateAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await movieService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
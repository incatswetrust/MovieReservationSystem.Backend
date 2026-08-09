using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.DTOs.Movie;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<PagedResult<MovieReadDto>>> GetAll(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var movies = await movieService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(movies);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<MovieReadDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var movie = await movieService.GetByIdAsync(id, cancellationToken);
        if (movie == null) return NotFound();
        return Ok(movie);
    }

    [AllowAnonymous]
    [HttpGet("genre/{genre}")]
    public async Task<ActionResult<IEnumerable<MovieReadDto>>> GetByGenre(string genre, CancellationToken cancellationToken)
    {
        var movies = await movieService.GetByGenreAsync(genre, cancellationToken);
        return Ok(movies);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<MovieReadDto>> Create(MovieCreateDto dto, CancellationToken cancellationToken)
    {
        var created = await movieService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<MovieReadDto>> Update(int id, MovieUpdateDto dto, CancellationToken cancellationToken)
    {
        var updated = await movieService.UpdateAsync(id, dto, cancellationToken);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var success = await movieService.DeleteAsync(id, cancellationToken);
        if (!success) return NotFound();
        return NoContent();
    }
}

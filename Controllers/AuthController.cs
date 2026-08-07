using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs.User;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;



[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService, IConfiguration config) : ControllerBase
{

    [Authorize(Roles = "User,Admin")]
    [HttpGet("status")]
    public async Task<ActionResult<UserReadDto>> Status()
    {
        var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
        var user = await userService.GetByIdAsync(int.Parse(userIdStr));
        if (user == null) return NotFound();
        return Ok(user);
    }
    [HttpPost("register")]
    public async Task<ActionResult<UserReadDto>> Register(UserRegisterDto dto)
    {
        var (userRead, refreshToken) = await userService.RegisterAsync(dto);
        var secretKey = config["JwtSettings:SecretKey"];
        var token = userService.GenerateJwtToken(userRead, secretKey);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // на prod обычно true
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(2)
        };
        Response.Cookies.Append("X-Access-Token", token, cookieOptions);
        Response.Cookies.Append("X-Refresh-Token", refreshToken, RefreshTokenCookieOptions());
        return Ok(userRead);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserReadDto>> Login(UserLoginDto dto)
    {
        var result = await userService.LoginAsync(dto);
        if (result == null)
            return Unauthorized("Invalid username or password.");
        var (userRead, refreshToken) = result.Value;
        var secretKey = config["JwtSettings:SecretKey"];
        var token = userService.GenerateJwtToken(userRead, secretKey);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // на prod обычно true
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(2)
        };
        Response.Cookies.Append("X-Access-Token", token, cookieOptions);
        Response.Cookies.Append("X-Refresh-Token", refreshToken, RefreshTokenCookieOptions());

        return Ok(userRead);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<UserReadDto>> Refresh()
    {
        var refreshToken = Request.Cookies["X-Refresh-Token"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        var result = await userService.RefreshTokenAsync(refreshToken);
        if (result == null)
            return Unauthorized();

        var (userRead, newRefreshToken) = result.Value;
        var secretKey = config["JwtSettings:SecretKey"];
        var token = userService.GenerateJwtToken(userRead, secretKey);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // на prod обычно true
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(2)
        };
        Response.Cookies.Append("X-Access-Token", token, cookieOptions);
        Response.Cookies.Append("X-Refresh-Token", newRefreshToken, RefreshTokenCookieOptions());

        return Ok(userRead);
    }

    private static CookieOptions RefreshTokenCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = false, // на prod обычно true
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddDays(7)
    };

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["X-Refresh-Token"];
        if (!string.IsNullOrEmpty(refreshToken))
            await userService.RevokeRefreshTokenAsync(refreshToken);

        Response.Cookies.Delete("X-Access-Token");
        Response.Cookies.Delete("X-Refresh-Token");
        return Ok("Logged out");
    }
}
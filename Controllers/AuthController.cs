using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs.User;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService, IConfiguration config) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        try
        {
            var userRead = await userService.RegisterAsync(dto);
            return Ok(userRead);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var userRead = await userService.LoginAsync(dto);
        if (userRead == null)
            return Unauthorized("Invalid username or password.");
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

        return Ok(userRead); 
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("X-Access-Token");
        return Ok("Logged out");
    }
}
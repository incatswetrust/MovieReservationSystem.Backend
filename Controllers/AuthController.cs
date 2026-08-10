using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.DTOs.User;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Controllers;



[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService, IConfiguration config, IWebHostEnvironment env) : ControllerBase
{

    [Authorize(Roles = "User,Admin")]
    [HttpGet("status")]
    public async Task<ActionResult<UserReadDto>> Status(CancellationToken cancellationToken)
    {
        var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
        var user = await userService.GetByIdAsync(int.Parse(userIdStr), cancellationToken);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPut("profile")]
    public async Task<ActionResult<UserReadDto>> UpdateProfile(UserUpdateDto dto, CancellationToken cancellationToken)
    {
        var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new ErrorResponse("User is not authenticated"));

        var updated = await userService.UpdateProfileAsync(int.Parse(userIdStr), dto, cancellationToken);
        if (updated == null) return NotFound(new ErrorResponse("User not found"));
        return Ok(updated);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserReadDto>> Register(UserRegisterDto dto, CancellationToken cancellationToken)
    {
        var (userRead, refreshToken) = await userService.RegisterAsync(dto, cancellationToken);
        var secretKey = config["JwtSettings:SecretKey"];
        var token = userService.GenerateJwtToken(userRead, secretKey);
        var cookieOptions = AccessTokenCookieOptions();
        Response.Cookies.Append("X-Access-Token", token, cookieOptions);
        Response.Cookies.Append("X-Refresh-Token", refreshToken, RefreshTokenCookieOptions());
        return Ok(userRead);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserReadDto>> Login(UserLoginDto dto, CancellationToken cancellationToken)
    {
        var result = await userService.LoginAsync(dto, cancellationToken);
        if (result == null)
            return Unauthorized(new ErrorResponse("Invalid username or password."));
        var (userRead, refreshToken) = result.Value;
        var secretKey = config["JwtSettings:SecretKey"];
        var token = userService.GenerateJwtToken(userRead, secretKey);
        var cookieOptions = AccessTokenCookieOptions();
        Response.Cookies.Append("X-Access-Token", token, cookieOptions);
        Response.Cookies.Append("X-Refresh-Token", refreshToken, RefreshTokenCookieOptions());

        return Ok(userRead);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<UserReadDto>> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["X-Refresh-Token"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        var result = await userService.RefreshTokenAsync(refreshToken, cancellationToken);
        if (result == null)
            return Unauthorized();

        var (userRead, newRefreshToken) = result.Value;
        var secretKey = config["JwtSettings:SecretKey"];
        var token = userService.GenerateJwtToken(userRead, secretKey);
        var cookieOptions = AccessTokenCookieOptions();
        Response.Cookies.Append("X-Access-Token", token, cookieOptions);
        Response.Cookies.Append("X-Refresh-Token", newRefreshToken, RefreshTokenCookieOptions());

        return Ok(userRead);
    }

    private CookieOptions AccessTokenCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = !env.IsDevelopment(),
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddHours(2)
    };

    private CookieOptions RefreshTokenCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = !env.IsDevelopment(),
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddDays(7)
    };

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["X-Refresh-Token"];
        if (!string.IsNullOrEmpty(refreshToken))
            await userService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);

        Response.Cookies.Delete("X-Access-Token");
        Response.Cookies.Delete("X-Refresh-Token");
        return Ok("Logged out");
    }

    [AllowAnonymous]
    [HttpGet("google")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback));
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [AllowAnonymous]
    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback(CancellationToken cancellationToken)
    {
        var authResult = await HttpContext.AuthenticateAsync("External");
        if (!authResult.Succeeded || authResult.Principal == null)
            return Unauthorized();

        var email = authResult.Principal.FindFirstValue(ClaimTypes.Email);
        var googleId = authResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        await HttpContext.SignOutAsync("External");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleId))
            return Unauthorized();

        var (userRead, refreshToken) = await userService.FindOrCreateGoogleUserAsync(email, googleId, cancellationToken);
        var secretKey = config["JwtSettings:SecretKey"];
        var token = userService.GenerateJwtToken(userRead, secretKey);
        var cookieOptions = AccessTokenCookieOptions();
        Response.Cookies.Append("X-Access-Token", token, cookieOptions);
        Response.Cookies.Append("X-Refresh-Token", refreshToken, RefreshTokenCookieOptions());

        var frontendUrl = config["Cors:ProdOrigin"];
        return Redirect(string.IsNullOrWhiteSpace(frontendUrl) ? "http://localhost:5173" : frontendUrl);
    }
}

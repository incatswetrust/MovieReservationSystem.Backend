using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.Mapping;
using MovieReservationSystem.Backend.Middleware;
using MovieReservationSystem.Backend.Services;
using MovieReservationSystem.Backend.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicy = "DevCors";
const string ExternalAuthScheme = "External";

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
});


builder.Services.AddSwaggerGen(options =>
{
   
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Movie Reservation API", Version = "v1" });
});

var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"];
var keyBytes = Encoding.UTF8.GetBytes(jwtSecretKey);

var authenticationBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };


    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Cookies["X-Access-Token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

// Google's OAuth handler is a request-handler scheme: UseAuthentication() probes it on
// every request (to match its callback path), which throws if ClientId/ClientSecret are
// missing. Only register it when both are actually configured, so an unconfigured Google
// login doesn't take down the entire API.
if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
{
    authenticationBuilder
        .AddCookie(ExternalAuthScheme)
        .AddGoogle(options =>
        {
            options.ClientId = googleClientId;
            options.ClientSecret = googleClientSecret;
            options.SignInScheme = ExternalAuthScheme;
        });
}

builder.Services.AddAuthorization();

var prodOrigin = builder.Configuration["Cors:ProdOrigin"];
var allowedOrigins = new List<string> { "http://localhost:5173" };
if (!string.IsNullOrWhiteSpace(prodOrigin))
{
    allowedOrigins.Add(prodOrigin);
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        policy.WithOrigins(allowedOrigins.ToArray())
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var details = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(
                e => e.Key,
                e => e.Value!.Errors.Select(err => err.ErrorMessage).ToArray()
            );

        return new BadRequestObjectResult(new ErrorResponse("Validation failed", details));
    };
});

const string AnonymousRateLimitPolicy = "AnonymousRateLimit";
const string AuthenticatedRateLimitPolicy = "AuthenticatedRateLimit";

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Global limiter: 60 req/min per anonymous client (by IP), 120 req/min per authenticated user.
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var isAuthenticated = httpContext.User.Identity?.IsAuthenticated ?? false;

        if (isAuthenticated)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? httpContext.User.Identity?.Name
                         ?? "authenticated";

            return RateLimitPartition.GetFixedWindowLimiter($"user:{userId}", _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
        }

        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter($"anon:{ip}", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 60,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });

    // Named policies available for [EnableRateLimiting("...")] on specific endpoints if needed.
    options.AddPolicy(AnonymousRateLimitPolicy, httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter($"anon:{ip}", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 60,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });

    options.AddPolicy(AuthenticatedRateLimitPolicy, httpContext =>
    {
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? httpContext.User.Identity?.Name
                     ?? "authenticated";

        return RateLimitPartition.GetFixedWindowLimiter($"user:{userId}", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 120,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });
});

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddSingleton<IGoogleAuthExchangeStore, GoogleAuthExchangeStore>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ICinemaService, CinemaService>();
builder.Services.AddScoped<IHallService, HallService>();
builder.Services.AddScoped<IHallImageService, HallImageService>();
builder.Services.AddScoped<IShowtimeService, ShowtimeService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ISeatService, SeatService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
 
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie Reservation API v1");
    });
}


app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseCors(CorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();


app.Run();

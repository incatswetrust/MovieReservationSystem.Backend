using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Mapping;
using MovieReservationSystem.Backend.Services;
using MovieReservationSystem.Backend.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});


builder.Services.AddSwaggerGen(options =>
{
   
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Movie Reservation API", Version = "v1" });
});

var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"];
var keyBytes = Encoding.UTF8.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(options =>
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

builder.Services.AddAuthorization(); 

builder.Services.AddControllers();


builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ICinemaService, CinemaService>();
builder.Services.AddScoped<IHallService, HallService>();
builder.Services.AddScoped<IShowtimeService, ShowtimeService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ISeatService, SeatService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
 
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie Reservation API v1");
    });
}


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


app.Run();

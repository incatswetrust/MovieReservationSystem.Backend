# Movie Reservation System — Backend

A movie reservation system API built with ASP.NET Core 8 and Entity Framework Core 9. It powers ticket booking for movies, cinemas, halls, and showtimes, with JWT + Google OAuth authentication, role-based access control, and a small set of production-grade hardening (rate limiting, pagination, standardized error responses).

Live API: `https://moviereservationsystem.runasp.net`
Frontend: [movie-reservation-frontend](https://github.com/incatswetrust/movie-reservation-frontend)

## Features

- **Auth**: register/login with BCrypt-hashed passwords, JWT access tokens in an HttpOnly cookie, rotating refresh tokens, Google OAuth sign-in
- **Roles**: `User`, `Admin` (full access), `Viewer` (read-only access to the admin panel — can view everything, can't create/edit/delete anything). Viewer accounts can only be created by an existing Admin (`POST /api/Users`), never via self-registration
- **Movies**: CRUD, search by title/director, filter by genre/year/rating, pagination
- **Cinemas & Halls**: CRUD, seat layouts, hall image galleries
- **Showtimes**: CRUD, lookup by movie/hall, availability by date
- **Bookings**: seat selection with availability checks, cancellation
- **Hardening**: global rate limiting (60 req/min anonymous, 120 req/min authenticated), `AsNoTracking()` on read paths, standardized `{ error, details }` error responses, CORS locked to configured origins

## Tech Stack

- ASP.NET Core 8 (Web API)
- Entity Framework Core 9 + SQL Server
- AutoMapper
- BCrypt.Net-Next for password hashing
- JWT Bearer + Google OAuth authentication
- Swashbuckle (Swagger — Development environment only)

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/incatswetrust/MovieReservationSystem.Backend.git
```

### 2. Run a local SQL Server instance

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 --name mrs-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

### 3. Configure secrets

Set the connection string, a JWT signing key (32+ characters), and — optionally — Google OAuth credentials via `dotnet user-secrets` (or `appsettings.Development.json`, which is gitignored):

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=moviereservation;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
dotnet user-secrets set "JwtSettings:SecretKey" "<32+ character random string>"
dotnet user-secrets set "Authentication:Google:ClientId" "<optional>"
dotnet user-secrets set "Authentication:Google:ClientSecret" "<optional>"
```

Google OAuth is only registered when both values are present — the app runs fine without them, Google sign-in is just unavailable.

### 4. Apply migrations and run

```bash
dotnet ef database update
dotnet run
```

The API listens on `http://localhost:5256` by default (see `Properties/launchSettings.json`).

### 5. Swagger UI

In the `Development` environment, browse to `/swagger` to explore and test endpoints. Swagger is disabled outside Development.

## License

Apache License 2.0 — see [LICENSE](./LICENSE).

## Security

See [SECURITY.md](./SECURITY.md) for how to report a vulnerability.

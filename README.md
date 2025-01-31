# Movie Reservation System
A Movie Reservation System built with C#, ASP.NET Core, Entity Framework Core, PostgreSQL, and Docker (for the database). The application features JWT-based authentication (stored in an HttpOnly cookie), user roles (Admin and User), and a complete booking workflow for movies, theaters, halls, showtimes, and seats.

## Main Features
### 1. User Registration and Login
* JWT authentication with the token stored in an HttpOnly cookie
* Admin and standard User roles
### 2. Movie Management
* CRUD operations for movies, including genre-based filtering
### 3. Cinema & Hall Management
* CRUD operations for multiple cinemas, halls, and seat layouts
### 4. Showtime Management
* CRUD operations for scheduling movies in specific halls
* Pricing per showtime
### 5. Booking
* Users can book seats for a chosen showtime
* Reserved seats are stored in a separate table (BookedSeats)
* Includes checks for seat availability
### 6. Admin Controls
* Admin can manage all data, including users, bookings, and movie details
## Technology Stack
C# + ASP.NET Core (Web API)
Entity Framework Core for data access
PostgreSQL (recommended via Docker)
AutoMapper for object mapping
Swagger for API documentation/testing
## Getting Started
### 1. Clone the Repository
```bash
git clone https://github.com/your-username/movie-reservation-system.git
```
### 2. Configure Database
* Use Docker to run PostgreSQL (see `docker-compose.yml` if provided, or run PostgreSQL locally).
* Update the `ConnectionStrings:DefaultConnection` in `appsettings.json`.
### 3. Run Migrations
```bash
dotnet ef database update
```
### 4. Launch the Application
```bash
dotnet run
```
The API should be available at `https://localhost:5001` or `http://localhost:5000`.
### 5. Swagger UI
Navigate to `/swagger` to explore and test endpoints.


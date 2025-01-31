using AutoMapper;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs.Booking;
using MovieReservationSystem.Backend.DTOs.Cinema;
using MovieReservationSystem.Backend.DTOs.Hall;
using MovieReservationSystem.Backend.DTOs.Movie;
using MovieReservationSystem.Backend.DTOs.Seat;
using MovieReservationSystem.Backend.DTOs.Showtime;
using MovieReservationSystem.Backend.DTOs.User;

namespace MovieReservationSystem.Backend.Mapping;

public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ===== Movie =====
            CreateMap<Movie, MovieReadDto>();           // Domain -> ReadDto
            CreateMap<MovieCreateDto, Movie>();         // CreateDto -> Domain
            CreateMap<MovieUpdateDto, Movie>();         // UpdateDto -> Domain

            // ===== Cinema =====
            CreateMap<Cinema, CinemaReadDto>();
            CreateMap<CinemaCreateDto, Cinema>();
            CreateMap<CinemaUpdateDto, Cinema>();

            // ===== Hall =====
            CreateMap<Hall, HallReadDto>();
            CreateMap<HallCreateDto, Hall>();
            CreateMap<HallUpdateDto, Hall>();
            CreateMap<HallCreateDto, Hall>();
            CreateMap<Hall, HallReadDto>()
                .ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.Seats));

            CreateMap<Seat, SeatReadDto>()
                .ForMember(dest => dest.IsReserved, opt => opt.Ignore());

            // ===== Showtime =====
            CreateMap<Showtime, ShowtimeReadDto>();
            CreateMap<ShowtimeCreateDto, Showtime>();
            CreateMap<ShowtimeUpdateDto, Showtime>();

            // ===== Booking =====
            CreateMap<Booking, BookingReadDto>()
                .ForMember(dest => dest.SeatIds, opt => opt.Ignore()); 
            
            CreateMap<BookingCreateDto, Booking>()
                .ForMember(dest => dest.BookedSeats, opt => opt.Ignore()); 
            

            CreateMap<BookingUpdateDto, Booking>();

            // ===== User =====
            CreateMap<User, UserReadDto>(); 

            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

        }
    }
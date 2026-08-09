using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs.HallImage;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

public class HallImageService(AppDbContext context, IMapper mapper) : IHallImageService
{
    public async Task<IEnumerable<HallImageReadDto>> GetByHallIdAsync(int hallId)
    {
        var images = await context.HallImages
            .AsNoTracking()
            .Where(i => i.HallId == hallId)
            .OrderBy(i => i.Order)
            .ToListAsync();
        return mapper.Map<IEnumerable<HallImageReadDto>>(images);
    }

    public async Task<HallImageReadDto> CreateAsync(int hallId, HallImageCreateDto dto)
    {
        var image = mapper.Map<HallImage>(dto);
        image.HallId = hallId;
        context.HallImages.Add(image);
        await context.SaveChangesAsync();
        return mapper.Map<HallImageReadDto>(image);
    }

    public async Task<bool> DeleteAsync(int hallId, int imageId)
    {
        var image = await context.HallImages
            .FirstOrDefaultAsync(i => i.Id == imageId && i.HallId == hallId);
        if (image == null) return false;

        context.HallImages.Remove(image);
        await context.SaveChangesAsync();
        return true;
    }
}

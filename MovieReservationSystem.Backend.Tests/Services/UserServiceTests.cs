using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.Services;
using MovieReservationSystem.Backend.Tests.TestHelpers;
using Xunit;

namespace MovieReservationSystem.Backend.Tests.Services;

/// <summary>
/// Covers UserService.DeleteAsync's last-admin guard (added this session to stop the app
/// from being locked out of its own admin console). Uses the EF Core InMemory provider —
/// DeleteAsync only does FindAsync/Remove/SaveChanges, none of which need a relational
/// provider, so InMemory is sufficient and keeps these tests fast.
/// </summary>
public class UserServiceTests
{
    private static UserService CreateService(AppDbContext context) =>
        new(context, DbContextFactory.CreateMapper());

    [Fact]
    public async Task DeleteAsync_LastRemainingAdmin_ThrowsAndDoesNotDelete()
    {
        using var context = DbContextFactory.CreateInMemory();
        var admin = new User { Username = "admin1", Role = UserRole.Admin };
        context.Users.Add(admin);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var ex = await Assert.ThrowsAsync<Exception>(() => service.DeleteAsync(admin.Id, CancellationToken.None));
        Assert.Contains("last remaining admin", ex.Message, StringComparison.OrdinalIgnoreCase);

        Assert.Equal(1, await context.Users.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_AdminWithAnotherAdminPresent_Succeeds()
    {
        using var context = DbContextFactory.CreateInMemory();
        var admin1 = new User { Username = "admin1", Role = UserRole.Admin };
        var admin2 = new User { Username = "admin2", Role = UserRole.Admin };
        context.Users.AddRange(admin1, admin2);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.DeleteAsync(admin1.Id, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(1, await context.Users.CountAsync());
        Assert.Null(await context.Users.FindAsync(admin1.Id));
    }

    [Theory]
    [InlineData(0)] // no admins at all
    [InlineData(1)] // one admin present, but deleting a regular user shouldn't care either way
    public async Task DeleteAsync_RegularUser_SucceedsRegardlessOfAdminCount(int adminCount)
    {
        using var context = DbContextFactory.CreateInMemory();
        for (var i = 0; i < adminCount; i++)
        {
            context.Users.Add(new User { Username = $"admin{i}", Role = UserRole.Admin });
        }

        var regularUser = new User { Username = "regular", Role = UserRole.User };
        context.Users.Add(regularUser);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.DeleteAsync(regularUser.Id, CancellationToken.None);

        Assert.True(result);
        Assert.Null(await context.Users.FindAsync(regularUser.Id));
        Assert.Equal(adminCount, await context.Users.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_UnknownId_ReturnsFalse()
    {
        using var context = DbContextFactory.CreateInMemory();
        var service = CreateService(context);

        var result = await service.DeleteAsync(id: 999, CancellationToken.None);

        Assert.False(result);
    }
}

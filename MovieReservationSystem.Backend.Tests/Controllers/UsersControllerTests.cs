using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Backend.Controllers;
using MovieReservationSystem.Backend.DTOs;
using MovieReservationSystem.Backend.DTOs.User;
using MovieReservationSystem.Backend.Services.Interfaces;
using Xunit;

namespace MovieReservationSystem.Backend.Tests.Controllers;

/// <summary>
/// Covers UsersController.Delete's admin self-delete guard. This check lives in the
/// controller (reading the caller's own id out of the JWT claims), not the service, so it
/// can only be exercised by testing the controller directly.
///
/// We picked a hand-rolled fake IUserService over a mocking library (no mocking package is
/// referenced by this repo yet, and the interface is small enough that a fake is simpler
/// than adding a new dependency for one test class) plus a manually-built ControllerContext
/// with a fabricated ClaimsPrincipal to stand in for an authenticated request.
/// </summary>
public class UsersControllerTests
{
    private static UsersController CreateController(IUserService userService, int callerUserId)
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, callerUserId.ToString()) },
            authenticationType: "TestAuth"));

        return new UsersController(userService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            }
        };
    }

    [Fact]
    public async Task Delete_SelfDelete_ReturnsBadRequestAndNeverCallsService()
    {
        var fakeService = new FakeUserService();
        var controller = CreateController(fakeService, callerUserId: 42);

        var result = await controller.Delete(id: 42, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var error = Assert.IsType<ErrorResponse>(badRequest.Value);
        Assert.Equal("You cannot delete your own account.", error.Error);
        Assert.False(fakeService.DeleteWasCalled);
    }

    [Fact]
    public async Task Delete_OtherUser_DelegatesToServiceAndReturnsNoContent()
    {
        var fakeService = new FakeUserService { DeleteResult = true };
        var controller = CreateController(fakeService, callerUserId: 42);

        var result = await controller.Delete(id: 7, CancellationToken.None);

        Assert.IsType<NoContentResult>(result.Result);
        Assert.True(fakeService.DeleteWasCalled);
        Assert.Equal(7, fakeService.LastDeletedId);
    }

    [Fact]
    public async Task Delete_OtherUserNotFound_ReturnsNotFound()
    {
        var fakeService = new FakeUserService { DeleteResult = false };
        var controller = CreateController(fakeService, callerUserId: 42);

        var result = await controller.Delete(id: 999, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    /// <summary>Minimal hand-rolled fake — only DeleteAsync is exercised by these tests.</summary>
    private sealed class FakeUserService : IUserService
    {
        public bool DeleteResult { get; set; } = true;
        public bool DeleteWasCalled { get; private set; }
        public int? LastDeletedId { get; private set; }

        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            DeleteWasCalled = true;
            LastDeletedId = id;
            return Task.FromResult(DeleteResult);
        }

        public string GenerateJwtToken(UserReadDto user, string secretKey) => throw new NotImplementedException();
        public Task<(UserReadDto User, string RefreshToken)> RegisterAsync(UserRegisterDto dto, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<(UserReadDto User, string RefreshToken)?> LoginAsync(UserLoginDto dto, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<IEnumerable<UserReadDto>> GetAllAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<UserReadDto?> GetByIdAsync(int id, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<string> GenerateRefreshTokenAsync(int userId, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<(UserReadDto User, string RefreshToken)?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<(UserReadDto User, string RefreshToken)> FindOrCreateGoogleUserAsync(string email, string googleId, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<UserReadDto?> UpdateProfileAsync(int userId, UserUpdateDto dto, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<UserReadDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}

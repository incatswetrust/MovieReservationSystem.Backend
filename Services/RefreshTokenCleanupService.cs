using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;

namespace MovieReservationSystem.Backend.Services;

/// <summary>
/// Periodically deletes expired or revoked refresh tokens so the table doesn't grow
/// forever — nothing else in the app ever removes a row from RefreshTokens.
/// </summary>
public class RefreshTokenCleanupService(
    IServiceScopeFactory scopeFactory,
    ILogger<RefreshTokenCleanupService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(6);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);

        do
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var now = DateTime.UtcNow;
                var deleted = await context.RefreshTokens
                    .Where(rt => rt.ExpiresAt < now || rt.IsRevoked)
                    .ExecuteDeleteAsync(stoppingToken);

                if (deleted > 0)
                {
                    logger.LogInformation("Removed {Count} expired/revoked refresh tokens", deleted);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Refresh token cleanup pass failed");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}

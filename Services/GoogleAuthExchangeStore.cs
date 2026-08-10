using System.Collections.Concurrent;
using System.Security.Cryptography;
using MovieReservationSystem.Backend.Services.Interfaces;

namespace MovieReservationSystem.Backend.Services;

/// <summary>
/// Short-lived, single-use codes that hand a Google OAuth callback off to the frontend's
/// own origin, so the session cookie gets set via the proxied /api call the frontend makes
/// (same origin as every other cookie in the app) instead of directly on the backend's own
/// domain, which the browser wouldn't send back through the frontend's proxy.
/// </summary>
public class GoogleAuthExchangeStore : IGoogleAuthExchangeStore
{
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);

    private readonly ConcurrentDictionary<string, (int UserId, DateTime ExpiresAt)> codes = new();

    public string Issue(int userId)
    {
        RemoveExpired();

        var code = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        codes[code] = (userId, DateTime.UtcNow.Add(Ttl));
        return code;
    }

    public bool TryConsume(string code, out int userId)
    {
        RemoveExpired();

        if (codes.TryRemove(code, out var entry) && entry.ExpiresAt >= DateTime.UtcNow)
        {
            userId = entry.UserId;
            return true;
        }

        userId = 0;
        return false;
    }

    private void RemoveExpired()
    {
        var now = DateTime.UtcNow;
        foreach (var (key, value) in codes)
        {
            if (value.ExpiresAt < now)
            {
                codes.TryRemove(key, out _);
            }
        }
    }
}

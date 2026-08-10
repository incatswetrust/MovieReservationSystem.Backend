namespace MovieReservationSystem.Backend.Services.Interfaces;

public interface IGoogleAuthExchangeStore
{
    string Issue(int userId);
    bool TryConsume(string code, out int userId);
}

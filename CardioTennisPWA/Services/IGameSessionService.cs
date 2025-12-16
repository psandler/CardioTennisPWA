using CardioTennisPWA.Models;

namespace CardioTennisPWA.Services;

public interface IGameSessionService
{
    GameSession? CurrentGameSession { get; }
    Task CreateGameSessionAsync(int numCourts, int numPlayers);
    Task GenerateInitialTeamsAsync();
    Task RemixTeamsAsync();
    Task EndGameSessionAsync();
}

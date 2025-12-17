using CardioTennisPWA.Models;

namespace CardioTennisPWA.Services;

public class GameSessionQueryService : IGameSessionQueryService
{
    private readonly ILocalStorageService _localStorage;
    private const string GameSessionKey = "cardiotennis:gamesession";

    public GameSessionQueryService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<GameSession?> GetCurrentGameSessionAsync()
    {
        return await _localStorage.GetItemAsync<GameSession>(GameSessionKey);
    }

    public async Task<MatchSet?> GetCurrentSetAsync()
    {
        var gameSession = await GetCurrentGameSessionAsync();
        if (gameSession == null || gameSession.Sets.Count == 0)
            return null;
            
        return gameSession.Sets
            .FirstOrDefault(s => s.Number == gameSession.CurrentSetNumber);
    }

    public async Task<Match?> GetCurrentMatchAsync()
    {
        var currentSet = await GetCurrentSetAsync();
        if (currentSet == null || currentSet.Matches.Count == 0)
            return null;
            
        return currentSet.Matches.LastOrDefault(m => !m.IsComplete) 
               ?? currentSet.Matches.Last();
    }

    public async Task<MatchSet?> GetSetAsync(int setNumber)
    {
        var gameSession = await GetCurrentGameSessionAsync();
        return gameSession?.Sets.FirstOrDefault(s => s.Number == setNumber);
    }

    public async Task<List<Player>> GetPlayerStandingsAsync()
    {
        var gameSession = await GetCurrentGameSessionAsync();
        if (gameSession == null)
            return new List<Player>();
            
        return gameSession.Players
            .OrderByDescending(p => p.Score)
            .ThenBy(p => p.Number)
            .ToList();
    }

    public async Task<bool> HasActiveGameSessionAsync()
    {
        var gameSession = await GetCurrentGameSessionAsync();
        return gameSession != null && gameSession.IsActive;
    }
}

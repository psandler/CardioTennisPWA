using CardioTennisPWA.Models;

namespace CardioTennisPWA.Services;

/// <summary>
/// Query service for read-only game session operations
/// All queries load fresh from localStorage
/// </summary>
public interface IGameSessionQueryService
{
    /// <summary>
    /// Gets the current active game session from localStorage
    /// </summary>
    Task<GameSession?> GetCurrentGameSessionAsync();
    
    /// <summary>
    /// Gets the current active set
    /// </summary>
    Task<MatchSet?> GetCurrentSetAsync();
    
    /// <summary>
    /// Gets the current active match
    /// </summary>
    Task<Match?> GetCurrentMatchAsync();
    
    /// <summary>
    /// Gets a specific set by number
    /// </summary>
    Task<MatchSet?> GetSetAsync(int setNumber);
    
    /// <summary>
    /// Gets player standings ordered by score descending
    /// </summary>
    Task<List<Player>> GetPlayerStandingsAsync();
    
    /// <summary>
    /// Checks if a game session is currently active
    /// </summary>
    Task<bool> HasActiveGameSessionAsync();
}

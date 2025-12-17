using CardioTennisPWA.Models;

namespace CardioTennisPWA.Services;

/// <summary>
/// Command service for state-modifying game session operations
/// </summary>
public interface IGameSessionCommandService
{
    /// <summary>
    /// Creates a new game session
    /// </summary>
    Task CreateGameSessionAsync(int numCourts, int numPlayers);
    
    /// <summary>
    /// Generates initial team assignments for the first set
    /// </summary>
    Task GenerateInitialTeamsAsync();
    
    /// <summary>
    /// Remixes teams for the first match (only allowed before first match starts)
    /// </summary>
    Task RemixTeamsAsync();
    
    /// <summary>
    /// Records the winning team for a specific court in the current match
    /// </summary>
    Task RecordMatchResultAsync(int courtNumber, int winningTeam);
    
    /// <summary>
    /// Continues with current teams (creates next match in same set, rotates courts)
    /// </summary>
    Task ContinueWithCurrentTeamsAsync();
    
    /// <summary>
    /// Ends current set and creates new set with regenerated teams
    /// </summary>
    Task ReassignTeamsAsync();
    
    /// <summary>
    /// Ends the current game session
    /// </summary>
    Task EndGameSessionAsync();
}

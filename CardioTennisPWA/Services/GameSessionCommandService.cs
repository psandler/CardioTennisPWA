using CardioTennisPWA.Models;

namespace CardioTennisPWA.Services;

public class GameSessionCommandService : IGameSessionCommandService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IGameSessionQueryService _queryService;
    private const string GameSessionKey = "cardiotennis:gamesession";

    public GameSessionCommandService(
        ILocalStorageService localStorage,
        IGameSessionQueryService queryService)
    {
        _localStorage = localStorage;
        _queryService = queryService;
    }

    public async Task CreateGameSessionAsync(int numCourts, int numPlayers)
    {
        var gameSession = new GameSession
        {
            NumCourts = numCourts,
            NumPlayers = numPlayers,
            IsActive = true,
            StartTime = DateTime.Now,
            CurrentSetNumber = 1
        };

        // Create players
        for (int i = 1; i <= numPlayers; i++)
        {
            gameSession.Players.Add(new Player { Number = i });
        }

        await SaveGameSessionAsync(gameSession);
    }

    public async Task GenerateInitialTeamsAsync()
    {
        var gameSession = await _queryService.GetCurrentGameSessionAsync();
        if (gameSession == null) return;

        // Create first set with first match
        var firstSet = new MatchSet { Number = 1 };
        var firstMatch = new Match { Number = 1 };

        // Randomly assign players to courts and teams
        AssignPlayersToMatch(firstMatch, gameSession.Players, gameSession.NumCourts);

        firstSet.Matches.Add(firstMatch);
        gameSession.Sets.Add(firstSet);

        await SaveGameSessionAsync(gameSession);
    }

    public async Task RemixTeamsAsync()
    {
        var gameSession = await _queryService.GetCurrentGameSessionAsync();
        if (gameSession == null || gameSession.Sets.Count == 0) return;

        // Only allow remix on first match of first set before it starts
        var firstSet = gameSession.Sets[0];
        if (firstSet.Number != 1 || firstSet.Matches.Count != 1) return;
        if (firstSet.Matches[0].IsComplete) return;

        // Clear and regenerate first match
        firstSet.Matches.Clear();
        var firstMatch = new Match { Number = 1 };
        AssignPlayersToMatch(firstMatch, gameSession.Players, gameSession.NumCourts);
        firstSet.Matches.Add(firstMatch);

        await SaveGameSessionAsync(gameSession);
    }

    public async Task RecordMatchResultAsync(int courtNumber, int winningTeam)
    {
        var gameSession = await _queryService.GetCurrentGameSessionAsync();
        if (gameSession == null) return;

        // Find the current set in the game session
        var currentSet = gameSession.Sets.FirstOrDefault(s => s.Number == gameSession.CurrentSetNumber);
        if (currentSet == null || currentSet.Matches.Count == 0) return;

        // Find the current match (last incomplete match, or last match if all complete)
        var currentMatch = currentSet.Matches.LastOrDefault(m => !m.IsComplete) 
                          ?? currentSet.Matches.Last();
        if (currentMatch == null) return;

        // Find the court in this match
        var court = currentMatch.Courts.FirstOrDefault(c => c.Number == courtNumber);
        if (court == null) return;

        // Record winner
        court.WinningTeam = winningTeam;

        // Award points to winning team players
        var winningPlayers = winningTeam == 1 ? court.TeamA.PlayerNumbers : court.TeamB.PlayerNumbers;
        foreach (var playerNumber in winningPlayers)
        {
            var player = gameSession.Players.FirstOrDefault(p => p.Number == playerNumber);
            if (player != null)
            {
                player.Score++;
            }
        }

        // Check if all courts have results
        if (currentMatch.Courts.All(c => c.WinningTeam.HasValue))
        {
            currentMatch.IsComplete = true;
        }

        await SaveGameSessionAsync(gameSession);
    }

    public async Task ContinueWithCurrentTeamsAsync()
    {
        var gameSession = await _queryService.GetCurrentGameSessionAsync();
        if (gameSession == null) return;

        // Find the current set in the game session
        var currentSet = gameSession.Sets.FirstOrDefault(s => s.Number == gameSession.CurrentSetNumber);
        if (currentSet == null || currentSet.Matches.Count == 0) return;

        // Find the current match (should be the last match)
        var currentMatch = currentSet.Matches.Last();
        if (!currentMatch.IsComplete) return;

        // Create next match
        var nextMatch = new Match { Number = currentSet.Matches.Count + 1 };

        // Rotate courts (up and down the ladder)
        if (gameSession.NumCourts == 1)
        {
            // Single court: same teams, same court
            var previousCourt = currentMatch.Courts[0];
            nextMatch.Courts.Add(new Court
            {
                Number = 1,
                TeamA = new Team { PlayerNumbers = previousCourt.TeamA.PlayerNumbers.ToList() },
                TeamB = new Team { PlayerNumbers = previousCourt.TeamB.PlayerNumbers.ToList() }
            });
        }
        else
        {
            // Multiple courts: winners move up, losers move down
            var sortedCourts = currentMatch.Courts.OrderBy(c => c.Number).ToList();
            var winners = new List<Team>();
            var losers = new List<Team>();

            foreach (var court in sortedCourts)
            {
                if (court.WinningTeam == 1)
                {
                    winners.Add(court.TeamA);
                    losers.Add(court.TeamB);
                }
                else
                {
                    winners.Add(court.TeamB);
                    losers.Add(court.TeamA);
                }
            }

            // Assign winners to top courts, losers to bottom courts
            for (int i = 0; i < gameSession.NumCourts; i++)
            {
                var newCourt = new Court { Number = i + 1 };
                
                if (i < winners.Count / 2)
                {
                    // Top courts: winner vs winner
                    newCourt.TeamA = new Team { PlayerNumbers = winners[i * 2].PlayerNumbers.ToList() };
                    newCourt.TeamB = new Team { PlayerNumbers = winners[i * 2 + 1].PlayerNumbers.ToList() };
                }
                else
                {
                    // Bottom courts: loser vs loser
                    int loserIndex = (i - winners.Count / 2) * 2;
                    newCourt.TeamA = new Team { PlayerNumbers = losers[loserIndex].PlayerNumbers.ToList() };
                    if (loserIndex + 1 < losers.Count)
                    {
                        newCourt.TeamB = new Team { PlayerNumbers = losers[loserIndex + 1].PlayerNumbers.ToList() };
                    }
                }
                
                nextMatch.Courts.Add(newCourt);
            }
        }

        currentSet.Matches.Add(nextMatch);
        await SaveGameSessionAsync(gameSession);
    }

    public async Task ReassignTeamsAsync()
    {
        var gameSession = await _queryService.GetCurrentGameSessionAsync();
        if (gameSession == null) return;

        // Mark current set as complete
        var currentSet = gameSession.Sets.FirstOrDefault(s => s.Number == gameSession.CurrentSetNumber);
        if (currentSet != null)
        {
            currentSet.IsComplete = true;
        }

        // Create new set with new match
        var newSetNumber = gameSession.Sets.Count + 1;
        var newSet = new MatchSet { Number = newSetNumber };
        var newMatch = new Match { Number = 1 };

        // Use skill-based balancing: rank players by score, then snake draft
        AssignTeamsBySkillBalance(newMatch, gameSession.Players, gameSession.NumCourts);

        newSet.Matches.Add(newMatch);
        gameSession.Sets.Add(newSet);
        gameSession.CurrentSetNumber = newSetNumber;

        await SaveGameSessionAsync(gameSession);
    }

    /// <summary>
    /// Assigns players to teams using skill-based balancing (snake draft)
    /// Players are ranked by score, then distributed to balance team strength
    /// </summary>
    private void AssignTeamsBySkillBalance(Match match, List<Player> players, int numCourts)
    {
        var random = new Random();
        
        // Sort players by score (descending), then random tiebreaker
        var rankedPlayers = players
            .OrderByDescending(p => p.Score)
            .ThenBy(p => random.Next())
            .ToList();

        // Create teams (2 per court)
        int totalTeams = numCourts * 2;
        var teams = new List<List<int>>();
        for (int i = 0; i < totalTeams; i++)
        {
            teams.Add(new List<int>());
        }

        // Snake draft: 1->2->3->4->4->3->2->1->1->2...
        bool forward = true;
        int teamIndex = 0;

        foreach (var player in rankedPlayers)
        {
            teams[teamIndex].Add(player.Number);

            // Move to next team
            if (forward)
            {
                teamIndex++;
                if (teamIndex >= totalTeams)
                {
                    teamIndex = totalTeams - 1;
                    forward = false;
                }
            }
            else
            {
                teamIndex--;
                if (teamIndex < 0)
                {
                    teamIndex = 0;
                    forward = true;
                }
            }
        }

        // Assign teams to courts
        for (int courtNum = 1; courtNum <= numCourts; courtNum++)
        {
            var court = new Court { Number = courtNum };
            
            int teamAIndex = (courtNum - 1) * 2;
            int teamBIndex = teamAIndex + 1;

            court.TeamA.PlayerNumbers = teams[teamAIndex];
            if (teamBIndex < teams.Count)
            {
                court.TeamB.PlayerNumbers = teams[teamBIndex];
            }

            match.Courts.Add(court);
        }
    }

    /// <summary>
    /// Assigns players to match randomly with balanced team sizes
    /// </summary>
    private void AssignPlayersToMatch(Match match, List<Player> players, int numCourts)
    {
        var availablePlayers = players.Select(p => p.Number).ToList();
        var random = new Random();
        
        // Shuffle players
        availablePlayers = availablePlayers.OrderBy(_ => random.Next()).ToList();

        int playersPerCourt = players.Count / numCourts;
        int extraPlayers = players.Count % numCourts;
        int playerIndex = 0;

        for (int courtNum = 1; courtNum <= numCourts; courtNum++)
        {
            var court = new Court { Number = courtNum };
            
            // Determine how many players on this court
            int courtPlayerCount = playersPerCourt + (courtNum <= extraPlayers ? 1 : 0);
            int teamASize = courtPlayerCount / 2;
            int teamBSize = courtPlayerCount - teamASize;

            // Assign players to Team A
            for (int i = 0; i < teamASize; i++)
            {
                court.TeamA.PlayerNumbers.Add(availablePlayers[playerIndex++]);
            }

            // Assign players to Team B
            for (int i = 0; i < teamBSize; i++)
            {
                court.TeamB.PlayerNumbers.Add(availablePlayers[playerIndex++]);
            }

            match.Courts.Add(court);
        }
    }

    public async Task EndGameSessionAsync()
    {
        var gameSession = await _queryService.GetCurrentGameSessionAsync();
        if (gameSession != null)
        {
            gameSession.IsActive = false;
            await SaveGameSessionAsync(gameSession);
        }
    }

    private async Task SaveGameSessionAsync(GameSession gameSession)
    {
        await _localStorage.SetItemAsync(GameSessionKey, gameSession);
    }
}

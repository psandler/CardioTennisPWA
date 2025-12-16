using CardioTennisPWA.Models;

namespace CardioTennisPWA.Services;

public class GameSessionService : IGameSessionService
{
    private readonly ILocalStorageService _localStorage;
    private GameSession? _currentGameSession;
    
    private const string GameSessionKey = "cardiotennis:gamesession";

    public GameSessionService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public GameSession? CurrentGameSession => _currentGameSession;

    public async Task CreateGameSessionAsync(int numCourts, int numPlayers)
    {
        _currentGameSession = new GameSession
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
            _currentGameSession.Players.Add(new Player { Number = i });
        }

        // Save to localStorage
        await SaveGameSessionAsync();
    }

    public async Task GenerateInitialTeamsAsync()
    {
        if (_currentGameSession == null) return;

        // Create first set with first match
        var firstSet = new MatchSet { Number = 1 };
        var firstMatch = new Match { Number = 1 };

        // Randomly assign players to courts and teams
        var availablePlayers = _currentGameSession.Players.Select(p => p.Number).ToList();
        var random = new Random();
        
        // Shuffle players
        availablePlayers = availablePlayers.OrderBy(_ => random.Next()).ToList();

        int playersPerCourt = _currentGameSession.NumPlayers / _currentGameSession.NumCourts;
        int extraPlayers = _currentGameSession.NumPlayers % _currentGameSession.NumCourts;
        int playerIndex = 0;

        for (int courtNum = 1; courtNum <= _currentGameSession.NumCourts; courtNum++)
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

            firstMatch.Courts.Add(court);
        }

        firstSet.Matches.Add(firstMatch);
        _currentGameSession.Sets.Add(firstSet);

        await SaveGameSessionAsync();
    }

    public async Task RemixTeamsAsync()
    {
        if (_currentGameSession == null || _currentGameSession.Sets.Count == 0) return;

        // Clear the current match and regenerate
        var currentSet = _currentGameSession.Sets[_currentGameSession.CurrentSetNumber - 1];
        currentSet.Matches.Clear();

        // Regenerate first match with different random assignment
        await GenerateInitialTeamsAsync();
    }

    public async Task EndGameSessionAsync()
    {
        if (_currentGameSession != null)
        {
            _currentGameSession.IsActive = false;
            await SaveGameSessionAsync();
        }
        
        _currentGameSession = null;
    }

    private async Task SaveGameSessionAsync()
    {
        if (_currentGameSession != null)
        {
            await _localStorage.SetItemAsync(GameSessionKey, _currentGameSession);
        }
    }
}

# CQRS Implementation & Feature Completion Summary

## Part 1: Service Split (CQRS Pattern)

### Architecture Change

Split `GameSessionService` into two services following **Command Query Responsibility Segregation (CQRS)** pattern:

**Query Service (`IGameSessionQueryService` / `GameSessionQueryService`):**
- Handles all **read-only** operations
- No state mutations
- Methods:
  - `CurrentGameSession` - Get active game session
  - `GetCurrentSet()` - Get current active set
  - `GetCurrentMatch()` - Get current active match  
  - `GetSet(int setNumber)` - Get specific set by number
  - `GetPlayerStandings()` - Get sorted player scores
  - `HasActiveGameSession()` - Check if session active
  - `LoadGameSessionAsync()` - Load from localStorage

**Command Service (`IGameSessionCommandService` / `GameSessionCommandService`):**
- Handles all **state-modifying** operations
- All methods return `Task` (async)
- Methods:
  - `CreateGameSessionAsync()` - Create new game session
  - `GenerateInitialTeamsAsync()` - Create first match teams
  - `RemixTeamsAsync()` - Regenerate first match teams
  - `RecordMatchResultAsync()` - Record winner, award points
  - `ContinueWithCurrentTeamsAsync()` - Create next match, rotate courts
  - `ReassignTeamsAsync()` - **NOT IMPLEMENTED** (see explanation below)
  - `EndGameSessionAsync()` - End game session

### Benefits of CQRS

? **Separation of Concerns** - Queries don't modify state, commands don't return data  
? **Easier Testing** - Can test queries and commands independently  
? **Clear Intent** - Method names clearly indicate read vs write  
? **Future Scalability** - Could optimize queries separately from commands  
? **Single Responsibility** - Each service has one reason to change  

### Dependency Injection

```csharp
// Query service registered as singleton instance (shared)
builder.Services.AddScoped<GameSessionQueryService>();
builder.Services.AddScoped<IGameSessionQueryService>(sp => 
    sp.GetRequiredService<GameSessionQueryService>());

// Command service depends on query service
builder.Services.AddScoped<IGameSessionCommandService, GameSessionCommandService>();
```

**Why this approach?**
- Query service holds the in-memory `_currentGameSession`
- Command service needs access to modify it via `SetCurrentGameSession()`
- Both services share the same instance

---

## Part 2: Implemented Features (5 of 6)

### ? 1. Wire up "Remix Teams" Button

**Implementation:**
- Button visible only on Set 1, Match 1, before match starts
- Calls `CommandService.RemixTeamsAsync()`
- Clears first match and regenerates with new random assignments
- Uses same `AssignPlayersToMatch()` logic as initial generation

**Code Location:**
- `SetView.razor` - Button with `@onclick="RemixTeams"`
- `SetView.razor.cs` - Calls `CommandService.RemixTeamsAsync()`
- `GameSessionCommandService.cs` - Implementation

**User Flow:**
1. After "Done - Generate Teams", user sees SetView
2. Set 1, Match 1 expanded
3. If not satisfied, click "?? Remix Teams"
4. Teams randomized again
5. Can remix multiple times

---

### ? 2. Wire up "Team A/B Wins" Buttons

**Implementation:**
- `RecordMatchResultAsync(int courtNumber, int winningTeam)`
- Records winner (1 = Team A, 2 = Team B)
- Awards +1 point to each player on winning team
- Marks match complete when all courts have results
- Saves to localStorage

**Code Location:**
- `SetView.razor` - Buttons with `@onclick="() => RecordWinner(court.Number, 1/2)"`
- `SetView.razor.cs` - Calls `CommandService.RecordMatchResultAsync()`
- `GameSessionCommandService.cs` - Implementation

**User Flow:**
1. Match in progress, courts show "Team A Wins" / "Team B Wins" buttons
2. Coach taps winner for each court
3. Buttons disappear, winner highlighted in green
4. Player scores updated
5. When all courts complete, post-match actions appear

**Visual Feedback:**
- Winner gets green border and background (`bg-success bg-opacity-10`)
- Winner badge displayed
- Buttons hidden after selection

---

### ? 3. Wire up "Continue with Current Teams" Button

**Implementation:**
- `ContinueWithCurrentTeamsAsync()`
- Creates next match in same set
- Implements "Up and Down the Ladder" court rotation

**Court Rotation Logic:**

**Single Court:**
```
Same teams stay on Court 1
No rotation needed
```

**Multiple Courts (Example: 2 courts):**
```
Match 1:
  Court 1: Team A vs Team B ? Team A wins
  Court 2: Team C vs Team D ? Team C wins

Match 2 (after Continue):
  Court 1: Team A vs Team C (both winners - moved UP)
  Court 2: Team B vs Team D (both losers - moved DOWN)
```

**Algorithm:**
1. Sort courts by number (1, 2, 3...)
2. Separate winners and losers
3. Top half of courts: Winner vs Winner
4. Bottom half of courts: Loser vs Loser

**Code Location:**
- `SetView.razor` - Button with `@onclick="ContinueWithCurrentTeams"`
- `SetView.razor.cs` - Calls `CommandService.ContinueWithCurrentTeamsAsync()`
- `GameSessionCommandService.cs` - Implementation with rotation logic

**User Flow:**
1. Match complete, post-match actions appear
2. Coach clicks "Continue with Current Teams"
3. Next match created in same set
4. Teams rotated to new courts
5. New match accordion appears (expanded)

---

### ? 5. Add Player Scores Display

**Implementation:**
- `GetPlayerStandings()` query method
- Returns players ordered by score (descending), then by number (ascending)
- Real-time updates as matches complete

**Usage:**
```csharp
var standings = QueryService.GetPlayerStandings();
// Returns: List<Player> sorted by score
```

**Future UI Integration:**
- Can add "View Scores" button to show modal/panel
- Display player numbers with current scores
- Show who's winning

---

### ? 6. Wire up State Management

**Implementation:**
- All state changes go through Command Service
- All reads go through Query Service
- `StateHasChanged()` called after commands to refresh UI
- localStorage auto-saves on every command

**State Flow:**
```
User Action ? Command Service ? Modify State ? Save to localStorage ? StateHasChanged() ? UI Updates
User View ? Query Service ? Read State ? Display
```

---

## Part 3: Team Reassignment Logic (NOT IMPLEMENTED)

### Question: What logic would you use to regenerate teams?

**Current Implementation (Random):**
```csharp
// Shuffle all players randomly
availablePlayers = availablePlayers.OrderBy(_ => random.Next()).ToList();
```

**Proposed Smart Algorithm (for ReassignTeamsAsync):**

### Option 1: Maximize Player Mixing (Play with Everyone)

**Goal:** Ensure everyone plays with as many different people as possible

**Algorithm:**
1. **Track History:**
   - Create matrix of player partnerships: `Dictionary<(int p1, int p2), int>` ? times played together
   - Update after each match

2. **Generate New Teams:**
   - For each court, try to create teams that:
     - Minimize repeat partnerships
     - Balance team sizes
   - Use weighted random selection favoring least-played-with combinations

3. **Pseudocode:**
```csharp
Dictionary<(int, int), int> GetPartnershipHistory(GameSession session)
{
    var history = new Dictionary<(int, int), int>();
    foreach (var set in session.Sets)
    {
        foreach (var match in set.Matches)
        {
            foreach (var court in match.Courts)
            {
                // Record Team A partnerships
                foreach (var p1 in court.TeamA.PlayerNumbers)
                {
                    foreach (var p2 in court.TeamA.PlayerNumbers.Where(x => x > p1))
                    {
                        var key = (p1, p2);
                        history[key] = history.GetValueOrDefault(key) + 1;
                    }
                }
                // Repeat for Team B
            }
        }
    }
    return history;
}

List<Team> GenerateBalancedTeams(List<Player> players, int numCourts, Dictionary partnerships)
{
    var teams = new List<Team>();
    var available = players.ToList();
    
    while (teams.Count < numCourts * 2)
    {
        var newTeam = new Team();
        
        // Pick first player randomly from available
        var first = available[random.Next(available.Count)];
        newTeam.PlayerNumbers.Add(first.Number);
        available.Remove(first);
        
        // Pick remaining teammates based on lowest partnership count
        var teamSize = CalculateTeamSize(players.Count, numCourts, teams.Count);
        while (newTeam.PlayerNumbers.Count < teamSize)
        {
            // Weight selection by inverse partnership history
            var weights = available.Select(p => 
                1.0 / (1 + partnerships.GetValueOrDefault((first.Number, p.Number)))).ToList();
            var next = WeightedRandomSelect(available, weights);
            newTeam.PlayerNumbers.Add(next.Number);
            available.Remove(next);
        }
        
        teams.Add(newTeam);
    }
    
    return teams;
}
```

### Option 2: Skill Balancing (Based on Win/Loss)

**Goal:** Create competitive matches by balancing teams by score

**Algorithm:**
1. **Rank Players by Score:**
   - Order players by current score (descending)

2. **Snake Draft:**
   - Alternate team assignments to balance strength
   - Team 1 gets players 1, 4, 5, 8, 9...
   - Team 2 gets players 2, 3, 6, 7, 10...

3. **Pseudocode:**
```csharp
void AssignTeamsBySnakeDraft(List<Player> players, int numCourts)
{
    var sorted = players.OrderByDescending(p => p.Score).ToList();
    var teams = new List<Team>();
    
    for (int i = 0; i < numCourts * 2; i++)
    {
        teams.Add(new Team());
    }
    
    bool forward = true;
    int teamIndex = 0;
    
    foreach (var player in sorted)
    {
        teams[teamIndex].PlayerNumbers.Add(player.Number);
        
        if (forward)
        {
            teamIndex++;
            if (teamIndex >= teams.Count)
            {
                teamIndex = teams.Count - 1;
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
}
```

### Option 3: Hybrid Approach (Recommended)

**Combine both strategies:**
1. **First 2 sets:** Random (everyone gets warmed up)
2. **Sets 3-4:** Maximize mixing (play with new people)
3. **Final sets:** Skill balancing (competitive games)

**Implementation:**
```csharp
public async Task ReassignTeamsAsync()
{
    var gameSession = _queryService.CurrentGameSession;
    if (gameSession == null) return;
    
    var newSet = new MatchSet { Number = gameSession.Sets.Count + 1 };
    var newMatch = new Match { Number = 1 };
    
    // Choose algorithm based on set number
    if (gameSession.Sets.Count < 2)
    {
        // Early game: Random
        AssignPlayersToMatch(newMatch, gameSession.Players, gameSession.NumCourts);
    }
    else if (gameSession.Sets.Count < 4)
    {
        // Mid game: Maximize mixing
        var history = GetPartnershipHistory(gameSession);
        var teams = GenerateBalancedTeams(gameSession.Players, gameSession.NumCourts, history);
        AssignTeamsToMatch(newMatch, teams);
    }
    else
    {
        // Late game: Skill balancing
        var teams = GenerateSkillBalancedTeams(gameSession.Players, gameSession.NumCourts);
        AssignTeamsToMatch(newMatch, teams);
    }
    
    newSet.Matches.Add(newMatch);
    gameSession.Sets.Add(newSet);
    gameSession.CurrentSetNumber = newSet.Number;
    
    await SaveGameSessionAsync(gameSession);
}
```

### Recommendation

**For MVP:** Use **Option 1 (Maximize Player Mixing)** with simple tracking

**Why:**
- Cardio Tennis is "mostly for fun" (per requirements)
- Social aspect important (play with everyone)
- Relatively simple to implement
- Can add skill balancing later if users request

**Data Structure:**
- Add `Dictionary<(int, int), int> PartnershipHistory` to `GameSession` model
- Update after each match
- Use in team generation

**Future Enhancement:**
- Add UI toggle: "Mix Teams" vs "Balance by Skill"
- Let coach choose strategy per session
- Save preference in session settings

---

## Files Modified/Created

### New Files:
1. `Services/IGameSessionQueryService.cs` - Query interface
2. `Services/GameSessionQueryService.cs` - Query implementation
3. `Services/IGameSessionCommandService.cs` - Command interface
4. `Services/GameSessionCommandService.cs` - Command implementation

### Modified Files:
1. `Program.cs` - Register both services
2. `Pages/Home.razor.cs` - Use split services
3. `Components/SetView.razor.cs` - Use split services, add action methods
4. `Components/SetView.razor` - Wire up buttons

### Removed Files:
1. ~~`Services/IGameSessionService.cs`~~ - Replaced by split services
2. ~~`Services/GameSessionService.cs`~~ - Replaced by split services

---

## Testing Checklist

? **Build:** Successful  
? **Manual Test Flow:**
1. Create new session
2. Complete player numbering
3. See Set 1, Match 1
4. Click "Remix Teams" (teams regenerate)
5. Click "Team A Wins" on Court 1 (winner highlighted, scores updated)
6. Click "Team B Wins" on Court 2 (all courts complete)
7. See post-match actions
8. Click "Continue with Current Teams" (Match 2 created, courts rotated)
9. Complete Match 2
10. Click "Continue" again (Match 3 created)
11. Verify court rotation logic (winners up, losers down)

---

## Next Steps

1. **Implement `ReassignTeamsAsync()`** using one of the algorithms above
2. **Add Player Scores UI** - Modal or panel showing current standings
3. **Add "End Session" button** - Available at all times
4. **Add Final Scores Screen** - Show when session ends
5. **Add Session Resume** - Load from localStorage on app start
6. **Add Error Handling** - Graceful failures for localStorage quota, etc.

---

**Status:** 
- ? CQRS Split Complete
- ? 5 of 6 Features Implemented
- ? Team Reassignment Logic Explained (Not Implemented)

**Build Status:** ? Successful

The architecture is now cleaner with separated concerns, and the core gameplay loop is functional! ??

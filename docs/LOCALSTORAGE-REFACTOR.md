# localStorage as Single Source of Truth - Refactoring

## Problem with Previous Approach

**Shared State Between Services:**
```csharp
// ? BAD: Shared in-memory state
public class GameSessionQueryService
{
    private GameSession? _currentGameSession; // Shared mutable state
    
    internal void SetCurrentGameSession(GameSession? gameSession)
    {
        _currentGameSession = gameSession; // Called by CommandService
    }
}
```

**Issues:**
- ? Tight coupling between Query and Command services
- ? Hidden dependency via `internal` method
- ? Risk of stale data (in-memory vs localStorage out of sync)
- ? Violates single responsibility (Query service manages state)
- ? Hard to reason about data flow

## New Approach: localStorage as Single Source of Truth

**Core Principle:** 
> **localStorage is the authoritative data store. Both services read from and write to it independently.**

### Architecture

```
???????????????????????????????????????????????
?           Browser localStorage              ?
?   Key: "cardiotennis:gamesession"          ?
?   Value: GameSession (JSON)                 ?
???????????????????????????????????????????????
           ?                      ?
           ? Read                 ? Write
           ?                      ?
    ?????????????????    ????????????????
    ? Query Service ?    ? Cmd Service  ?
    ?  (Read Only)  ?    ? (Read+Write) ?
    ?????????????????    ????????????????
```

### Query Service

**All methods load fresh from localStorage:**

```csharp
public class GameSessionQueryService : IGameSessionQueryService
{
    private readonly ILocalStorageService _localStorage;
    
    // No in-memory state!
    
    public async Task<GameSession?> GetCurrentGameSessionAsync()
    {
        return await _localStorage.GetItemAsync<GameSession>(GameSessionKey);
    }
    
    public async Task<MatchSet?> GetCurrentSetAsync()
    {
        var gameSession = await GetCurrentGameSessionAsync(); // Fresh load
        // ... compute and return
    }
    
    // All other queries follow same pattern
}
```

### Command Service

**Loads from localStorage, modifies, saves back:**

```csharp
public class GameSessionCommandService : IGameSessionCommandService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IGameSessionQueryService _queryService;
    
    // No in-memory state!
    
    public async Task RecordMatchResultAsync(int courtNumber, int winningTeam)
    {
        // 1. Load from localStorage
        var gameSession = await _queryService.GetCurrentGameSessionAsync();
        
        // 2. Modify
        var court = /* find court */;
        court.WinningTeam = winningTeam;
        
        // 3. Save back to localStorage
        await SaveGameSessionAsync(gameSession);
    }
}
```

### Component Usage

**Components load data on init and after commands:**

```csharp
public partial class SetView
{
    private GameSession? currentGameSession; // Local copy for UI binding
    
    protected override async Task OnInitializedAsync()
    {
        await LoadGameSessionAsync();
    }
    
    private async Task LoadGameSessionAsync()
    {
        // Load fresh from query service (which loads from localStorage)
        currentGameSession = await QueryService.GetCurrentGameSessionAsync();
    }
    
    private async Task RecordWinner(int courtNumber, int winningTeam)
    {
        // Execute command (reads from localStorage, modifies, saves)
        await CommandService.RecordMatchResultAsync(courtNumber, winningTeam);
        
        // Reload to get updated state
        await LoadGameSessionAsync();
    }
}
```

## Benefits of New Approach

? **Single Source of Truth** - localStorage is always authoritative  
? **No Shared State** - Services are independent  
? **Clear Data Flow** - Explicit load ? modify ? save  
? **Always Fresh** - Every query reads latest from localStorage  
? **Simple DI** - No complex service registration tricks  
? **Testable** - Mock localStorage, services don't depend on each other  
? **Predictable** - No hidden coupling or internal methods  

## Tradeoffs

**Performance:**
- Each query hits localStorage (JSON deserialization)
- For typical game session size (~100KB), this is negligible
- Browser localStorage operations are synchronous and fast

**Optimization (if needed later):**
- Add optional in-memory caching with TTL
- Use event bus for invalidation
- But keep localStorage as source of truth

## Data Flow Example

**User clicks "Team A Wins" on Court 1:**

```
1. SetView.RecordWinner(1, 1) called
   ?
2. CommandService.RecordMatchResultAsync(1, 1)
   ?
3. Query: gameSession = await localStorage.GetItemAsync()
   ?
4. Modify: court.WinningTeam = 1, player.Score++
   ?
5. Save: await localStorage.SetItemAsync(gameSession)
   ?
6. SetView.LoadGameSessionAsync()
   ?
7. Query: gameSession = await localStorage.GetItemAsync()
   ?
8. Bind: currentGameSession = gameSession
   ?
9. UI updates with new state
```

## Code Changes Summary

### Modified Interfaces

**IGameSessionQueryService:**
- ? Removed: `GameSession? CurrentGameSession { get; }`
- ? Added: `Task<GameSession?> GetCurrentGameSessionAsync()`
- ? All methods now async and return `Task<T>`

**IGameSessionCommandService:**
- No changes (already async)

### Modified Implementations

**GameSessionQueryService:**
- ? Removed: `private GameSession? _currentGameSession`
- ? Removed: `internal void SetCurrentGameSession()`
- ? All methods load from localStorage

**GameSessionCommandService:**
- ? Uses `_queryService.GetCurrentGameSessionAsync()` instead of internal method
- ? No dependency on shared state

**Program.cs (DI):**
```csharp
// Before: Complex shared instance registration
builder.Services.AddScoped<GameSessionQueryService>();
builder.Services.AddScoped<IGameSessionQueryService>(sp => 
    sp.GetRequiredService<GameSessionQueryService>());

// After: Simple independent registration
builder.Services.AddScoped<IGameSessionQueryService, GameSessionQueryService>();
builder.Services.AddScoped<IGameSessionCommandService, GameSessionCommandService>();
```

**SetView.razor.cs:**
- Added: `private GameSession? currentGameSession` (local copy for binding)
- Added: `LoadGameSessionAsync()` method
- Changed: `OnInitialized()` ? `OnInitializedAsync()` with load
- Changed: All action methods now reload after command

**SetView.razor:**
- Changed: Bind to `currentGameSession` instead of `QueryService.CurrentGameSession`

## Testing

**Build Status:** ? Successful (hot reload warnings are normal for signature changes)

**Manual Test:**
1. Create new game session
2. Verify data in localStorage (DevTools ? Application ? Local Storage)
3. Record match result
4. Verify localStorage updated
5. Refresh page (to test persistence)
6. Verify state restored correctly

## Future Considerations

**If performance becomes an issue:**
1. Add simple in-memory cache with timestamp
2. Invalidate cache after writes
3. Keep localStorage as source of truth for cross-tab consistency

**For now:**
- Simple is better
- localStorage reads are fast enough
- Always consistent
- Easy to debug

---

**Status:** ? Refactoring Complete  
**Architecture:** localStorage as single source of truth  
**Services:** Independent, no shared state  
**Data Flow:** Explicit load ? modify ? save  
**Complexity:** Reduced (simpler DI, no internal methods)  

This is a much cleaner architecture that follows SOLID principles! ???

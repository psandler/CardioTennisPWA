using CardioTennisPWA.Models;
using CardioTennisPWA.Services;
using Microsoft.AspNetCore.Components;

namespace CardioTennisPWA.Components;

public partial class SetView
{
    [Inject]
    private IGameSessionQueryService QueryService { get; set; } = default!;
    
    [Inject]
    private IGameSessionCommandService CommandService { get; set; } = default!;
    
    [Parameter]
    public EventCallback OnSessionEnded { get; set; }

    private GameSession? currentGameSession;
    private int selectedSetNumber = 1;
    private bool showScoresModal = false;
    private bool showFinalScoresModal = false;
    private bool showEndSessionConfirmation = false;
    private List<Player>? playerStandings;
    private bool isEndingSession = false;
    private int totalMatchesPlayed = 0;
    
    private MatchSet? selectedSet => currentGameSession?.Sets
        .FirstOrDefault(s => s.Number == selectedSetNumber);
    
    private bool canEndSession
    {
        get
        {
            if (currentGameSession == null) return false;
            
            // Find current match across all sets
            var currentSet = currentGameSession.Sets.FirstOrDefault(s => s.Number == currentGameSession.CurrentSetNumber);
            if (currentSet == null) return true;
            
            var currentMatch = currentSet.Matches.LastOrDefault(m => !m.IsComplete);
            if (currentMatch == null) return true;
            
            // Disable if any court has reported a winner but not all
            var courtsWithResults = currentMatch.Courts.Count(c => c.WinningTeam.HasValue);
            return courtsWithResults == 0 || courtsWithResults == currentMatch.Courts.Count;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadGameSessionAsync();
    }

    private async Task LoadGameSessionAsync()
    {
        currentGameSession = await QueryService.GetCurrentGameSessionAsync();
        
        // Select the current set by default (or first if current not found)
        if (currentGameSession?.Sets.Count > 0)
        {
            selectedSetNumber = currentGameSession.CurrentSetNumber;
        }
    }

    private void SelectSet(int setNumber)
    {
        selectedSetNumber = setNumber;
    }
    
    private async Task RemixTeams()
    {
        await CommandService.RemixTeamsAsync();
        await LoadGameSessionAsync();
    }
    
    private async Task RecordWinner(int courtNumber, int winningTeam)
    {
        await CommandService.RecordMatchResultAsync(courtNumber, winningTeam);
        await LoadGameSessionAsync();
    }
    
    private async Task ContinueWithCurrentTeams()
    {
        await CommandService.ContinueWithCurrentTeamsAsync();
        await LoadGameSessionAsync();
    }
    
    private async Task ReassignTeams()
    {
        await CommandService.ReassignTeamsAsync();
        await LoadGameSessionAsync();
        // Auto-select the newly created set
        if (currentGameSession != null)
        {
            selectedSetNumber = currentGameSession.CurrentSetNumber;
        }
    }
    
    private async Task ShowScores()
    {
        playerStandings = await QueryService.GetPlayerStandingsAsync();
        showScoresModal = true;
    }
    
    private void HideScoresModal()
    {
        showScoresModal = false;
    }
    
    private void ShowEndSessionConfirmation()
    {
        showEndSessionConfirmation = true;
    }
    
    private void CancelEndSession()
    {
        showEndSessionConfirmation = false;
    }
    
    private async Task ConfirmEndSession()
    {
        showEndSessionConfirmation = false;
        isEndingSession = true;
        
        try
        {
            // End the session
            await CommandService.EndGameSessionAsync();
            
            // Load final scores
            playerStandings = await QueryService.GetPlayerStandingsAsync();
            
            // Calculate total matches
            totalMatchesPlayed = currentGameSession?.Sets.Sum(s => s.Matches.Count) ?? 0;
            
            // Show final scores modal
            showFinalScoresModal = true;
        }
        finally
        {
            isEndingSession = false;
        }
    }
    
    private async Task CloseFinalScores()
    {
        showFinalScoresModal = false;
        await OnSessionEnded.InvokeAsync();
    }
}

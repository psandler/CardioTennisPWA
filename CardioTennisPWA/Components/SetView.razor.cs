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

    private GameSession? currentGameSession;
    private int selectedSetNumber = 1;
    private bool showScoresModal = false;
    private List<Player>? playerStandings;
    
    private MatchSet? selectedSet => currentGameSession?.Sets
        .FirstOrDefault(s => s.Number == selectedSetNumber);

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
}

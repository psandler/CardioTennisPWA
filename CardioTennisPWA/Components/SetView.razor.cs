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
    private MatchSet? selectedSet => currentGameSession?.Sets
        .FirstOrDefault(s => s.Number == selectedSetNumber);

    protected override async Task OnInitializedAsync()
    {
        await LoadGameSessionAsync();
    }

    private async Task LoadGameSessionAsync()
    {
        currentGameSession = await QueryService.GetCurrentGameSessionAsync();
        
        // Select the first set by default
        if (currentGameSession?.Sets.Count > 0)
        {
            selectedSetNumber = currentGameSession.Sets[0].Number;
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
}

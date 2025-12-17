using Microsoft.AspNetCore.Components;
using CardioTennisPWA.Services;

namespace CardioTennisPWA.Pages;

public partial class Home
{
    [Inject]
    private ILocalStorageService LocalStorage { get; set; } = default!;

    [Inject]
    private IGameSessionService GameSessionService { get; set; } = default!;

    private bool showSessionSetupModal = false;
    private bool showPlayerNumberingModal = false;
    private bool showSetView = false;
    private int numCourts = 2;
    private int numPlayers = 16; // Default: 8 per court with 2 courts
    
    private const int MinPlayersPerCourt = 4;
    private const int MaxPlayersPerCourt = 8;
    
    private int minPlayers => numCourts * MinPlayersPerCourt;
    private int maxPlayers => numCourts * MaxPlayersPerCourt;
    private bool canDecrementPlayers => numPlayers > minPlayers;
    private bool canIncrementPlayers => numPlayers < maxPlayers;
    
    private string? validationMessage;
    private bool isFormValid => validationMessage == null;
    private string playersPerCourtText = "";

    private void ShowNewSessionModal()
    {
        // Reset to defaults
        numCourts = 2;
        numPlayers = 16;
        ValidateForm();
        showSessionSetupModal = true;
    }

    private void HideSessionSetupModal()
    {
        showSessionSetupModal = false;
    }

    private void IncrementPlayers()
    {
        if (numPlayers < maxPlayers)
        {
            numPlayers++;
            ValidateForm();
        }
    }

    private void DecrementPlayers()
    {
        if (numPlayers > minPlayers)
        {
            numPlayers--;
            ValidateForm();
        }
    }

    private void ValidateForm()
    {
        validationMessage = null;
        
        if (numPlayers < minPlayers)
        {
            validationMessage = $"Minimum {minPlayers} players required ({MinPlayersPerCourt} per court × {numCourts} courts)";
        }
        else if (numPlayers > maxPlayers)
        {
            validationMessage = $"Maximum {maxPlayers} players allowed ({MaxPlayersPerCourt} per court × {numCourts} courts)";
        }
        else
        {
            // Calculate team distribution across all courts
            // Each court has 2 teams, so total teams = numCourts * 2
            int totalTeams = numCourts * 2;
            int basePlayersPerTeam = numPlayers / totalTeams;
            int teamsWithExtra = numPlayers % totalTeams;
            
            int teamsWithMorePlayers = teamsWithExtra;
            int teamsWithFewerPlayers = totalTeams - teamsWithExtra;
            
            if (teamsWithExtra == 0)
            {
                // All teams have same number of players
                playersPerCourtText = $"{totalTeams} teams with {basePlayersPerTeam} players each";
            }
            else
            {
                // Some teams have more players than others
                playersPerCourtText = $"{teamsWithMorePlayers} team(s) with {basePlayersPerTeam + 1} players, {teamsWithFewerPlayers} team(s) with {basePlayersPerTeam} players";
            }
        }
    }

    private async Task StartGameSession()
    {
        // Create game session in service
        await GameSessionService.CreateGameSessionAsync(numCourts, numPlayers);
        
        // Hide setup modal
        showSessionSetupModal = false;
        
        // Show player numbering instructions
        showPlayerNumberingModal = true;
    }

    private async Task PlayerNumberingComplete()
    {
        // Generate initial teams
        await GameSessionService.GenerateInitialTeamsAsync();
        
        // Hide instructions modal
        showPlayerNumberingModal = false;
        
        // Show set view
        showSetView = true;
    }
}

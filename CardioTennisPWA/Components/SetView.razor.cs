using CardioTennisPWA.Models;
using Microsoft.AspNetCore.Components;

namespace CardioTennisPWA.Components;

public partial class SetView
{
    private int selectedSetNumber = 1;
    private MatchSet? selectedSet => GameSessionService.CurrentGameSession?.Sets
        .FirstOrDefault(s => s.Number == selectedSetNumber);

    protected override void OnInitialized()
    {
        // Select the first set by default
        if (GameSessionService.CurrentGameSession?.Sets.Count > 0)
        {
            selectedSetNumber = GameSessionService.CurrentGameSession.Sets[0].Number;
        }
    }

    private void SelectSet(int setNumber)
    {
        selectedSetNumber = setNumber;
    }
}

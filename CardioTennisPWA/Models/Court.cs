namespace CardioTennisPWA.Models;

public class Court
{
    public int Number { get; set; }
    public Team TeamA { get; set; } = new();
    public Team TeamB { get; set; } = new();
    public int? WinningTeam { get; set; } // 1 for TeamA, 2 for TeamB
}

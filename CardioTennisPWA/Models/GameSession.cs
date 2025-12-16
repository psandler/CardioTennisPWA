namespace CardioTennisPWA.Models;

public class GameSession
{
    public int NumCourts { get; set; }
    public int NumPlayers { get; set; }
    public List<Player> Players { get; set; } = new();
    public List<MatchSet> Sets { get; set; } = new();
    public int CurrentSetNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartTime { get; set; }
}

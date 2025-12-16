namespace CardioTennisPWA.Models;

public class Team
{
    public List<int> PlayerNumbers { get; set; } = new();
    public int? WonMatchNumber { get; set; } // Track which match this team won (if any)
}

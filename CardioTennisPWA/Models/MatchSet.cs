namespace CardioTennisPWA.Models;

public class MatchSet
{
    public int Number { get; set; } // Set number in the session
    public List<Match> Matches { get; set; } = new();
    public bool IsComplete { get; set; }
}

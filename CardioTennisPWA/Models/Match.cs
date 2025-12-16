namespace CardioTennisPWA.Models;

public class Match
{
    public int Number { get; set; } // Match number within the set (1, 2, or 3)
    public List<Court> Courts { get; set; } = new();
    public bool IsComplete { get; set; }
}

namespace Service.Models;

public record ProofOfWorkDto
{ 
    public int SecondsUntilBlock { get; init; }

    public long TotalHashRate { get; set; } 

    public double Difficulty { get; set; }

    public double Expected { get; set; } 

    public string Threshold { get; set; } = "";
}
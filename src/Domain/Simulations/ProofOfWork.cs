namespace Domain.Simulations;

public class ProofOfWork : ISimulation
{
    public string SimulationType => "proofOfWork";

    public int SecondsUntilBlock { get; set; }

    public long? TotalHashRate { get; set; }

    public double? Difficulty { get; set; }

    public double? Expected { get; set; }

    public string ExpectedPrefix
    {
        get
        {
            var expected = Expected ?? 1;
            var iterations = 0;

            while (expected < 1)
            {
                iterations++;
                expected *= 10;
            }

            if (!(Math.Abs(expected - 1) > 0.1))
                return "".PadLeft(iterations, '0');

            var x = (int)Math.Round(expected, MidpointRounding.ToNegativeInfinity);
            var y = "0123456789abcdef";
            var suffix = y[x].ToString();
            
            iterations--;

            return "".PadLeft(iterations, '0') + suffix;
        }
    }
}
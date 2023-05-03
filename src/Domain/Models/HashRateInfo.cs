namespace Domain.Models;

public record HashRateInfo
{
    public double Difficulty { get; set; }
    
    public double HashRate { get; init; }

    public string BeautifiedHashRate => CalculateBeautified();

    private string CalculateBeautified()
    {
        var units = new[] { "", "kilo", "mega", "giga", "tera", "peta", "exa", "zetta" };
        var hashRate = HashRate;
        int i;

        for (i = 0; i < units.Length; i++)
        {
            if (hashRate < 1000)
                break;

            hashRate /= 1000;
        }

        return hashRate.ToString("F2", Defaults.CultureInfo) + units[i];
    }
}
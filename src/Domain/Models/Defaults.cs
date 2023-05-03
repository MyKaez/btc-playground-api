using System.Globalization;

namespace Domain.Models;

public static class Defaults
{
    public const int BlockDurationInSeconds = 600;
    
    public static readonly IFormatProvider CultureInfo = new CultureInfo("en-us");
}
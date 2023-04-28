namespace Domain.Models;

public record Price
{
    public string Currency { get; init; } = null!;

    public decimal CurrentPrice { get; init; }

    public decimal PreviousPrice { get; init; }
}
namespace Service.Models;

public record PriceDto
{
    public string Currency { get; init; } = null!;

    public decimal Price { get; init; }

    public decimal PreviousPrice { get; init; }
}
namespace Service.Models.Requests;

public record UserRequest
{
    public required string Name { get; init; }
}
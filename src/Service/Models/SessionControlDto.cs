namespace Service.Models;

public record SessionControlDto : SessionDto
{
    public Guid ControlId { get; init; }
}
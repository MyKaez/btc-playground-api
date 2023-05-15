namespace Service.Models;

public record UserControlDto : UserDto
{
    public Guid ControlId { get; init; }
}
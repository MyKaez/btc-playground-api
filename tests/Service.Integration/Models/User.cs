namespace Service.Integration.Models;

public record User
{
    public Guid Id { get; set; }

    public Guid ControlId { get; set; }

    public string Name { get; set; } = "";
}
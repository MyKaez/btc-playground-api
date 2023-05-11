namespace Service.Integration.Models;

public record SessionControl : Session
{
    public Guid ControlId { get; set; }
}
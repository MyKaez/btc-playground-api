using Service.Models;

namespace Service.Integration.Models;

public record UserControl
{
    public Guid Id { get; set; }

    public Guid ControlId { get; set; }
    
    public UserStatusDto Status { get; set; }

    public string Name { get; set; } = "";
}
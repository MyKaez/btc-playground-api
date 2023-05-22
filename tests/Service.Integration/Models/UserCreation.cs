using Domain.Models;

namespace Service.Integration.Models;

public record UserCreation
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";
    
    public UserStatus Status { get; set; }
}
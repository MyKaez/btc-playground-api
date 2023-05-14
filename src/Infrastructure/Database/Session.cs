namespace Infrastructure.Database;

public class Session
{
    public Guid Id { get; set; }
    
    public Guid ControlId { get; set; }

    public string Name { get; set; }

    public string Status { get; set; }
    
    public string? Configuration { get; set; }

    public DateTime Created { get; set; }

    public DateTime Updated { get; set; }

    public DateTime ExpiresAt { get; set; }
    
    public ICollection<Interaction> Interactions { get; set; }
    
    public ICollection<Message> Messages { get; set; }
}
namespace Infrastructure.Database;

public class Interaction
{
    public Guid Id { get; set; }
    
    public Guid SessionId { get; set; }

    public virtual Session Session { get; set; } 

    public Guid UserId { get; set; }
    
    public virtual User User { get; set; }
}
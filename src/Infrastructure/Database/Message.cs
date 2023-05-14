namespace Infrastructure.Database;

public class Message
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }

    public Session Session { get; set; }
    
    public Guid UserId { get; set; }

    public User User { get; set; }

    public string Text { get; set; }

    public DateTime Created { get; set; }
}
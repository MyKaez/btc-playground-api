namespace Infrastructure.Database;

public class Connection
{
    public string Id { get; set; }

    public Guid? SessionId { get; set; }

    public Session? Session { get; set; }

    public Guid? UserId { get; set; }

    public User? User { get; set; }
}
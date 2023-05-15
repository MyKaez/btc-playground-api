namespace Infrastructure.Database;

public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
namespace Infrastructure.Database;

public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    
    public ICollection<Interaction> Interactions { get; set; }
    
    public ICollection<Message> Messages { get; set; }
}
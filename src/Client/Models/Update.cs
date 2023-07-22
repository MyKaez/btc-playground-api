namespace Client.Models;

public class Update
{
    public Guid? Id { get; set; }

    public string? Status { get; set; }

    public string? Configuration { get; set; }
    
    public DateTime? StartTime { get; set; }
    
    public DateTime? EndTime { get; set; }
    
    public bool? Urgent { get; set; }
}
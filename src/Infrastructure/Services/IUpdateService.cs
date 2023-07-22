namespace Infrastructure.Services;

public interface IUpdateService
{
    void AddUpdate(Guid sessionId);
    
    void RemoveUpdate(Guid sessionId);
    
    IEnumerable<Guid> GetUpdates();
}
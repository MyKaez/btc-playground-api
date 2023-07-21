namespace Infrastructure.Services;

public interface IUpdateService
{
    void AddUpdate(Guid sessionId);
    
    IEnumerable<Guid> GetUpdates();
}
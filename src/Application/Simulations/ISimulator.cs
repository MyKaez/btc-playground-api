using System.Text.Json;
using Application.Models;
using Domain.Models;

namespace Application.Simulations;

public interface ISimulator
{
    Task<RequestResult<JsonElement>?> Prepare(Session session, CancellationToken cancellationToken);
    
    Task<RequestResult<JsonElement>?> Start(Session session, CancellationToken cancellationToken);
    
    Task<RequestResult<JsonElement>?> Stop(Session session, CancellationToken cancellationToken);
    
    Task<RequestResult<JsonElement>?> Reset(Session session, CancellationToken cancellationToken);
}
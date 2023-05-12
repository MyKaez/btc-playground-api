using System.Text.Json.Nodes;
using Domain.Models;

namespace Application.Services;

public interface IUserService
{
    User? GetById(Session session, Guid userId);
    
    Task<User> Create(Session session, string userName, CancellationToken cancellationToken);

    Task Execute(Session session, User user, JsonNode data, CancellationToken cancellationToken);
}
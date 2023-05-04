using Domain.Models;

namespace Application.Services;

public interface IUserService
{
    Task<User> Create(Session session, string userName, CancellationToken cancellationToken);
}
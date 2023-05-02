using Domain.Models;

namespace Application.Services;

public interface IUserService
{
    User Create(Session session, string userName);
}
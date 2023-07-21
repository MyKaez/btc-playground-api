using Application.Services;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IUpdateService _updateService;

    public UserService(IUserRepository userRepository, IMapper mapper, IUpdateService updateService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _updateService = updateService;
    }

    public async Task<User?> GetById(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(userId, cancellationToken);
        var res = _mapper.Map<User>(user);

        return res;
    }

    public async Task<User> Create(Session session, string userName, CancellationToken cancellationToken)
    {
        var user = new Database.User
        {
            Id = Guid.NewGuid(), ControlId = Guid.NewGuid(), Name = userName, Status = UserStatus.NotReady.ToString()
        };
        var res = _mapper.Map<User>(user);

        await _userRepository.Create(session.Id, user, cancellationToken);
        
        _updateService.AddUpdate(session.Id);

        return res;
    }

    public async Task<User?> Update(UserUpdate update, CancellationToken cancellationToken)
    {
        var user = await _userRepository.Update(
            update.UserId, user =>
            {
                user.Status = update.Status.ToString();
                user.Configuration = update.Configuration.ToString();
            }, cancellationToken);

        if (user is null)
            return null;

        var res = _mapper.Map<User>(user);

        _updateService.AddUpdate(update.SessionId);
        
        return res;
    }

    public async Task<User[]> GetBySessionId(Guid sessionId, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetBySessionId(sessionId, cancellationToken);
        var res = _mapper.Map<User[]>(users);

        return res;
    }
}
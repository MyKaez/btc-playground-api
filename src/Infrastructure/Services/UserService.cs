using Application.Services;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IUpdateService _updateService;
    private readonly IMemoryCache _memoryCache;

    public UserService(IUserRepository userRepository, IMapper mapper, IUpdateService updateService,
        IMemoryCache memoryCache)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _updateService = updateService;
        _memoryCache = memoryCache;
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
        var key = "users:" + sessionId;

        if (!_updateService.GetUpdates().Contains(sessionId) && _memoryCache.TryGetValue<User[]>(key, out var cached))
            return cached ?? Array.Empty<User>();

        var users = await _userRepository.GetBySessionId(sessionId, cancellationToken);
        var res = _mapper.Map<User[]>(users);

        _memoryCache.Set(key, res, TimeSpan.FromMinutes(1));

        return res;
    }
}
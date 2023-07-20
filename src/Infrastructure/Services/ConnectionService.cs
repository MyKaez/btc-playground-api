using AutoMapper;
using Infrastructure.Models;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public class ConnectionService : IConnectionService
{
    private readonly IConnectionRepository _connectionRepository;
    private readonly IMapper _mapper;

    public ConnectionService(IConnectionRepository connectionRepository, IMapper mapper)
    {
        _connectionRepository = connectionRepository;
        _mapper = mapper;
    }

    public async Task<ICollection<Connection>> GetAll()
    {
        var connections = await _connectionRepository.GetAll();
        var res = connections.Select(con => _mapper.Map<Connection>(con)).ToList();

        return res;
    }

    public async Task Add(string connectionId, Guid sessionId)
    {
        await _connectionRepository.Add(connectionId, sessionId);
    }

    public async Task Update(string connectionId, Guid userId)
    {
        await _connectionRepository.Update(connectionId, userId);
    }

    public async Task Remove(string connectionId)
    {
        await _connectionRepository.Remove(connectionId);
    }

    public async Task<Connection?> Get(string connectionId)
    {
        var connection = await _connectionRepository.Get(connectionId);
        var res = connection is null ? null : _mapper.Map<Connection>(connection);

        return res;
    }
}
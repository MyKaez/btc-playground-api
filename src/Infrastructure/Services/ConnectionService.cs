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

    public ICollection<Connection> GetAll()
    {
        var connections = _connectionRepository.GetAll();
        var res = connections.Select(con => _mapper.Map<Connection>(con)).ToList();

        return res;
    }

    public void Add(string connectionId, Guid sessionId)
    {
        _connectionRepository.Add(connectionId, sessionId);
    }

    public void Update(string connectionId, Guid userId)
    {
        _connectionRepository.Update(connectionId, userId);
    }

    public void Remove(string connectionId)
    {
        _connectionRepository.Remove(connectionId);
    }

    public Connection? Get(string connectionId)
    {
        var connection = _connectionRepository.Get(connectionId);
        var res = connection is null ? null : _mapper.Map<Connection>(connection);

        return res;
    }
}
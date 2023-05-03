using System.Net.Http.Json;
using AutoMapper;
using Domain.Models;

namespace Infrastructure.Mempool;

public class MempoolService
{
    public const string HttpClientFactoryName = "MEMPOOL_SERVICE";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMapper _mapper;

    public MempoolService(IHttpClientFactory httpClientFactory, IMapper mapper)
    {
        _httpClientFactory = httpClientFactory;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<Block>?> GetBlocks(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(HttpClientFactoryName);
        var blocks = await client.GetFromJsonAsync<MempoolBlock[]>("blocks", cancellationToken);
        var res = _mapper.Map<Block[]>(blocks);

        return res;
    }

    public async Task<HashRateInfo?> GetHashRateInfo(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(HttpClientFactoryName);
        var blocks = await client.GetFromJsonAsync<MempoolHashRate>("mining/hashrate/3d", cancellationToken);
        var res = _mapper.Map<HashRateInfo>(blocks);

        return res;
    }
}
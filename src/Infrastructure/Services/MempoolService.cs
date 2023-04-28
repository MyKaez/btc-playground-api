using System.Net.Http.Json;
using AutoMapper;
using Domain.Models;

namespace Infrastructure.Services;

public record MempoolConfig
{
    public string Url { get; init; } = null!;
}

public record MempoolBlock
{
    public string Id { get; init; } = null!;

    public long Height { get; init; }

    public long TimeStamp { get; init; }

    public ulong Nonce { get; init; }

    public double Difficulty { get; init; }
}

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
}
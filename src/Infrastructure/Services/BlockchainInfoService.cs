using System.Net.Http.Json;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using AutoMapper;
using Domain.Models;

namespace Infrastructure.Services;

public record BlockchainInfoConfig
{
    public string Url { get; init; } = null!;
}

public record BlockchainInfoPrice
{
    public string Currency { get; init; } = null!;

    public decimal Last { get; init; }

    [JsonPropertyName("15m")]
    public decimal _15M { get; init; }
}

public class BlockchainInfoService
{
    public const string HttpClientFactoryName = "BLOCKCHAIN_INFO_SERVICE";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMapper _mapper;

    public BlockchainInfoService(IHttpClientFactory httpClientFactory, IMapper mapper)
    {
        _httpClientFactory = httpClientFactory;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<Price>?> GetPrices(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(HttpClientFactoryName);
        var prices = await client.GetFromJsonAsync<Dictionary<string, BlockchainInfoPrice>>(
            "ticker", cancellationToken);

        if (prices is null)
            return null;

        var res = prices.Select(kvp => _mapper.Map<Price>(kvp.Value) with { Currency = kvp.Key }).ToArray();

        return res;
    }
}
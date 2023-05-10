using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Shared;

public class HttpClientFactory
{
    private const string BaseAddress = "http://localhost:5000";

    private readonly WebApplicationFactory<Program> _factory;

    public HttpClientFactory(IServiceCollection services)
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(host =>
        {
            host.UseKestrel();
            host.UseUrls(BaseAddress);
            host.UseContentRoot(Directory.GetCurrentDirectory());
            host.ConfigureServices(serviceCollection =>
            {
                foreach (var service in services.Where(ser =>
                             !serviceCollection.Select(s => s.ServiceType).Contains(ser.ServiceType)))
                    serviceCollection.Add(service);
            });
        });
    }

    public TestServer TestServer => _factory.Server;

    public HttpClient CreateClient()
    {
        var client = _factory.CreateClient();

        client.BaseAddress = new Uri(BaseAddress);

        return client;
    }
}
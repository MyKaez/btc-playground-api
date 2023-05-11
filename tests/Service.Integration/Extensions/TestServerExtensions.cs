using Microsoft.AspNetCore.TestHost;
using Service.Integration.Models;

namespace Service.Integration.Extensions;

public static class TestServerExtensions
{
    public static async Task<TestHub> CreateHub(this TestServer testServer, Session session)
    {
        var hubConnector = new HubConnector(testServer);
        var hub = await hubConnector.CreateConnection(session);

        return hub;
    }
}
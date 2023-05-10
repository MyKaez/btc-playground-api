using Microsoft.AspNetCore.SignalR.Client;

namespace Service.Integration.Models;

public record TestHub(HubConnection Connection, IReadOnlyCollection<string> Messages);
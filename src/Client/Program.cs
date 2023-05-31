using System.Diagnostics;
using System.Net.Http.Json;

var stop = TimeSpan.FromSeconds(60);
var i = 0;
var task = Task.Run(async () =>
{
    var stopwatch = Stopwatch.StartNew();
    var http = new HttpClient
    {
        BaseAddress = new Uri("https://api.btcis.me/v1/")
    };

    while (stopwatch.Elapsed < stop)
    {
        i++;
        await http.GetFromJsonAsync<BlockDto[]>("blocks");
    }
});
var tasks = new[] { task, task, task, task, task, task, task, task, task };

await Parallel.ForEachAsync(tasks, async (innerTask, _) => await innerTask);

Console.WriteLine($"Executed {i} requests in {stop}");

public class BlockDto
{
    public string Id { get; init; } = "";

    public long Height { get; init; }

    public DateTime TimeStamp { get; init; }

    public ulong Nonce { get; init; }

    public double Difficulty { get; init; }

    public double Probability { get; init; }
}
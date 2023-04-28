using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.PipelineBehaviours;

public class LogDuration<TIn, TOut> : IPipelineBehavior<TIn, TOut> where TIn : notnull
{
    private readonly ILogger<LogDuration<TIn, TOut>> _logger;

    public LogDuration(ILogger<LogDuration<TIn, TOut>> logger)
    {
        _logger = logger;
    }

    public async Task<TOut> Handle(TIn request, RequestHandlerDelegate<TOut> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await next();

        _logger.LogInformation(
            "Executed request {Name} in {StartNew} milliseconds",
            typeof(TIn).FullName, stopwatch.ElapsedMilliseconds);
        
        stopwatch.Stop();
        
        return result;
    }
}
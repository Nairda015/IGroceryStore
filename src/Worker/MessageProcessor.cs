using Microsoft.Extensions.Hosting;

namespace Worker;

public class MessageProcessor : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
        return Task.CompletedTask;
    }
}
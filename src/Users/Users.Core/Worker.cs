using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Users.Core;

public class Worker : BackgroundService
{
    private readonly IBus _bus;

    public Worker(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var message = new HelloMessage {Name = "World"};
        while (!stoppingToken.IsCancellationRequested)
        {
            await _bus.Publish(message, stoppingToken);
            await Task.Delay(1000, stoppingToken);
        }
    }
}

public class HelloMessage
{
    public string Name { get; set; }
}

public class TestConsumer : IConsumer<HelloMessage>
{
    private readonly ILogger<TestConsumer> _logger;

    public TestConsumer(ILogger<TestConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<HelloMessage> context)
    {
        _logger.LogInformation("{Date} Hello {Name}!", DateTime.Now.TimeOfDay.ToString(), context.Message.Name);
        return Task.CompletedTask;
    }
}
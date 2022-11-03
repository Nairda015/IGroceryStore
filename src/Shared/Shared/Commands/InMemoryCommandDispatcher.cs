using System.Diagnostics;
using IGroceryStore.Shared.Abstraction.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.Tracing;

namespace IGroceryStore.Shared.Commands;

internal sealed class InMemoryCommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryCommandDispatcher(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
    {
        var commandType = command.GetType();
        var sourceName = commandType.Assembly.FullName?.Split(new[] { ',', '.' }, 3)[1];
        using var source = new ActivitySource(sourceName ?? throw new EventSourceException());

        var activityName = $"Resolving {commandType.Name} command";
        using var activity = source.StartActivity(activityName);
        activity!.AddTag("command.name", commandType.Name);

        using var scope = _serviceProvider.CreateScope();
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod(nameof(ICommandHandler<ICommand<TResult>, TResult>.HandleAsync));

        if (method is null)
        {
            throw new InvalidOperationException($"Query handler for '{typeof(TResult).Name}' is invalid.");
        }

        return await (Task<TResult>)method.Invoke(handler, new object[] { command, cancellationToken });
    }
}

using System.Diagnostics;
using System.Diagnostics.Tracing;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Shared.Queries;

internal sealed class InMemoryQueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryQueryDispatcher(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        var queryType = query.GetType();
        var sourceName = queryType.Assembly.FullName?.Split(new[] { ',', '.' }, 3)[1];
        using var source = new ActivitySource(sourceName ?? throw new EventSourceException());

        var activityName = $"Resolving {queryType.Name} command";
        using var activity = source.StartActivity(activityName);
        activity!.AddTag("command.name", queryType.Name);

        using var scope = _serviceProvider.CreateScope();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync));
        if (method is null)
        {
            throw new InvalidOperationException($"Query handler for '{typeof(TResult).Name}' is invalid.");
        }

        return await (Task<TResult>)method.Invoke(handler, new object[] { query, cancellationToken });
    }
}

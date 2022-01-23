namespace IGroceryStore.Shared.Abstraction.Commands;

public interface ICommandDispatcher
{
     Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}
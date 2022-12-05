using IGroceryStore.Shared.Contracts;

namespace IGroceryStore.Shared.SQS;

public interface IMessageHandler
{
    public Task HandleAsync(IMessage message);
    public static abstract Type MessageType { get; }
}

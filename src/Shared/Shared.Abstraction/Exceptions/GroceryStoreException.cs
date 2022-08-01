using System.Net;

namespace IGroceryStore.Shared.Abstraction.Exceptions;

public abstract class GroceryStoreException : Exception
{
    public abstract HttpStatusCode StatusCode { get; }

    protected GroceryStoreException(string message) : base(message)
    {
    }
}

public abstract class GroceryConsumerException : Exception
{
    public bool ShouldBeRetried { get; }
    public Guid CorrelationId { get; }
    public uint RetryCount { get; set; }

    public string Message { get; }

    protected GroceryConsumerException(bool shouldBeRetried, Guid correlationId, string message)
        => (ShouldBeRetried, CorrelationId, Message) = (shouldBeRetried, correlationId, message);
}
using System.Net;

namespace IGroceryStore.Shared.Exceptions;

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
    public Guid? CorrelationId { get; }
    public uint RetryCount { get; set; }

    protected GroceryConsumerException(bool shouldBeRetried, Guid? correlationId, string message) :base(message)
        => (ShouldBeRetried, CorrelationId) = (shouldBeRetried, correlationId);
}

using Microsoft.AspNetCore.Http;

namespace IGroceryStore.Shared.EndpointBuilders;

public interface IHttpQuery { }

public interface IHttpCommand { }

public interface IHttpQueryHandler<in TQuery> where TQuery : IHttpQuery
{
    Task<IResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}

public interface IHttpCommandHandler<in TCommand> where TCommand : IHttpCommand
{
    Task<IResult> HandleAsync(TCommand query, CancellationToken cancellationToken);
}

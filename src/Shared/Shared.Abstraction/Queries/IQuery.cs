using Microsoft.AspNetCore.Http;

namespace IGroceryStore.Shared.Abstraction.Queries;

public interface IQuery<TResult>
{
    
}

public interface IHttpQuery : IQuery<IResult>
{
    
}
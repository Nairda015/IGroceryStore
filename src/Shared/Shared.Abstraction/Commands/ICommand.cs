using Microsoft.AspNetCore.Http;

namespace IGroceryStore.Shared.Abstraction.Commands;

public interface ICommand
{
    
}

public interface ICommand<TResult>
{
    
}

public interface IHttpCommand : ICommand<IResult>
{
    
}
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace IGroceryStore.Products.Core.Features.Products.Commands;

public record AddProduct() : ICommand<Guid>;

public class AddProductController : ApiControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public AddProductController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    public async Task<ActionResult<Guid>> Add([FromBody] AddProduct command)
    {
        var productId = await _dispatcher.DispatchAsync(command);
        return Ok(productId);
    }
}

internal class AddProductHandler : ICommandHandler<AddProduct, Guid>
{
    public Task<Guid> HandleAsync(AddProduct command, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Guid.NewGuid());
    }
}
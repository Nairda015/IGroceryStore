using IGroceryStore.Baskets.Core.Factories;
using IGroceryStore.Baskets.Core.Persistence;
using IGroceryStore.Shared.Abstraction.Commands;
using Microsoft.AspNetCore.Mvc;

namespace IGroceryStore.Baskets.Core.Features.Baskets;

public record AddBasket(string Name) : ICommand<Guid>;

public class AddBasketController : BasketsControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public AddBasketController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost("/baskets")]
    public async Task<ActionResult<Guid>> AddBasket([FromBody] AddBasket command, CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.DispatchAsync(command, cancellationToken);
        return Ok(result);
    }
}

internal class AddBasketHandler : ICommandHandler<AddBasket, Guid>
{
    private readonly IBasketFactory _factory;
    private readonly BasketDbContext _context;

    public AddBasketHandler(IBasketFactory factory, BasketDbContext dbContext)
    {
        _factory = factory;
        _context = dbContext;
    }

    public async Task<Guid> HandleAsync(AddBasket command, CancellationToken cancellationToken = default)
    {
        var basket = _factory.Create(command.Name);
        
        _context.Baskets.Add(basket);
        await _context.SaveChangesAsync(cancellationToken);
        return basket.Id;
    }
}
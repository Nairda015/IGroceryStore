using IGroceryStore.Baskets.Core.Factories;
using IGroceryStore.Baskets.Core.Persistence;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Baskets.Core.Features.Baskets;

public record AddBasket(string Name) : ICommand<Guid>;

public class AddBasketController : BasketsControllerBase, IEndpoint
{
    private readonly ICommandDispatcher _commandDispatcher;

    public AddBasketController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/baskets", async (AddBasket command, CancellationToken cancellationToken) =>
        {
            var result = await _commandDispatcher.DispatchAsync(command, cancellationToken);
            return Results.Ok(result);
        });
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
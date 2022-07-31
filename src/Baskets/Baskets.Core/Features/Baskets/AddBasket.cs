using IGroceryStore.Baskets.Core.Factories;
using IGroceryStore.Baskets.Core.Persistence;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Baskets.Core.Features.Baskets;

public record AddBasket(string Name) : ICommand<Guid>;

public class AddBasketEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/baskets", async(
            [FromServices] ICommandDispatcher dispatcher,
            AddBasket command,
            CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.DispatchAsync(command, cancellationToken);
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
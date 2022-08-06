using IGroceryStore.Baskets.Core.Factories;
using IGroceryStore.Baskets.Core.Persistence;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using Microsoft.AspNetCore.Routing;
using IGroceryStore.Shared.Abstraction.Constants;
using Microsoft.AspNetCore.Http;

namespace IGroceryStore.Baskets.Core.Features.Baskets;

public record AddBasket(string Name) : IHttpCommand;

public class AddBasketEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<AddBasket>("/basket").WithTags(SwaggerTags.Baskets);
}

internal class AddBasketHandler : ICommandHandler<AddBasket, IResult>
{
    private readonly IBasketFactory _factory;
    private readonly BasketDbContext _context;

    public AddBasketHandler(IBasketFactory factory, BasketDbContext dbContext)
    {
        _factory = factory;
        _context = dbContext;
    }

    public async Task<IResult> HandleAsync(AddBasket command, CancellationToken cancellationToken = default)
    {
        var basket = _factory.Create(command.Name);
        
        _context.Baskets.Add(basket);
        await _context.SaveChangesAsync(cancellationToken);
        return Results.Ok(basket.Id);
    }
}
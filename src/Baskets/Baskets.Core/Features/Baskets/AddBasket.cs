using IGroceryStore.Baskets.Core.Factories;
using IGroceryStore.Baskets.Core.Persistence;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using Microsoft.AspNetCore.Routing;
using IGroceryStore.Shared.Abstraction.Constants;
using Microsoft.AspNetCore.Http;

namespace IGroceryStore.Baskets.Core.Features.Baskets;

internal record AddBasket(AddBasket.AddBasketBody Body) : IHttpCommand
{
    public record AddBasketBody(string Name);
}

public class AddBasketEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<AddBasket>("api/basket").WithTags(SwaggerTags.Baskets);
}

internal class AddBasketHandler : ICommandHandler<AddBasket, IResult>
{
    private readonly IBasketFactory _factory;
    private readonly BasketsDbContext _context;

    public AddBasketHandler(IBasketFactory factory, BasketsDbContext dbContext)
    {
        _factory = factory;
        _context = dbContext;
    }

    public async Task<IResult> HandleAsync(AddBasket command, CancellationToken cancellationToken = default)
    {
        var basket = _factory.Create(command.Body.Name);
        
        _context.Baskets.Add(basket);
        await _context.SaveChangesAsync(cancellationToken);
        return Results.Ok(basket.Id);
    }
}

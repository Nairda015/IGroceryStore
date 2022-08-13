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
        endpoints.MapPost<AddBasketRequest>("/basket").WithTags(SwaggerTags.Baskets);
}

internal class AddBasketHandler : ICommandHandler<AddBasketRequest, IResult>
{
    private readonly IBasketFactory _factory;
    private readonly BasketDbContext _context;

    public AddBasketHandler(IBasketFactory factory, BasketDbContext dbContext)
    {
        _factory = factory;
        _context = dbContext;
    }

    public async Task<IResult> HandleAsync(AddBasketRequest request, CancellationToken cancellationToken = default)
    {
        var basket = _factory.Create(command.Body.Name);
        
        _context.Baskets.Add(basket);
        await _context.SaveChangesAsync(cancellationToken);
        return Results.Ok(basket.Id);
    }
}
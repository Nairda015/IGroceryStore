using IGroceryStore.Baskets.Core.Events;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Baskets.Core.Features.Baskets;

internal record AddProductsToBasket(AddProductsToBasket.AddProductsToBasketBody Body) : IHttpCommand
{
    
    public record AddProductsToBasketBody(Guid BasketId, ulong ProductId);
}

public class AddProductsToBasketEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<AddProductsToBasket>("/basket/{basketId:guid}/{productId:ulong}")
            .Produces(202)
            .WithTags(SwaggerTags.Baskets);
}


internal class AddProductsToBasketHandler : ICommandHandler<AddProductsToBasket, IResult>
{
    private readonly IDocumentSession _session;

    public AddProductsToBasketHandler(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<IResult> HandleAsync(AddProductsToBasket command, CancellationToken cancellationToken)
    {
        var (basketId, productId) = command.Body;
        
        var product = await _session.Events.FetchStreamStateAsync(productId.ToString(), cancellationToken);
        if (product is null) return Results.NotFound("Product not found"); //??

        _session.Events.Append(basketId, new ProductAddedToBasket(productId));
        await _session.SaveChangesAsync(cancellationToken);
        return Results.Accepted();
    }
}

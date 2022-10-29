using IGroceryStore.Baskets.Core.Projectors;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Queries;
using Marten;
using Marten.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Baskets.Core.Features.Products;

internal record GetPricesForShop(GetPricesForShop.GetPricesForShopBody Body) : IHttpQuery
{
    public record GetPricesForShopBody(ulong ShopId, ulong ProductId);
}

public class GetPricesForShopEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet<GetPricesForShop>("/basket/{shopId/{productId}}")
            .Produces<ProductProjectionForShop>()
            .WithTags(SwaggerTags.Baskets);
}

internal class AddProductsToBasketHandler : IQueryHandler<GetPricesForShop, IResult>
{
    private readonly IDocumentSession _session;

    public AddProductsToBasketHandler(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<IResult> HandleAsync(GetPricesForShop command, CancellationToken cancellationToken)
    {
        var (shopId, productId) = command.Body;
        var result =  await _session.Events
            .AggregateStreamAsync<ProductProjectionForShop>(productId.ToString(), token: cancellationToken);

        return Results.Ok(result);
    }
}




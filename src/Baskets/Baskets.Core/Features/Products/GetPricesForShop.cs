using IGroceryStore.Baskets.Core.Projectors;
using IGroceryStore.Baskets.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;

namespace IGroceryStore.Baskets.Core.Features.Products;

internal record GetPricesForShop(GetPricesForShop.GetPricesForShopBody Body) : IHttpQuery
{
    public record GetPricesForShopBody(ulong ProductId, ulong ShopId);
}

public class GetPricesForShopEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet<GetPricesForShop>("/basket/{shopId}/{productId}")
            .Produces<ProductProjectionForShop>()
            .WithTags(SwaggerTags.Baskets);
}

internal class AddProductsToBasketHandler : IQueryHandler<GetPricesForShop, IResult>
{
    private readonly IMongoCollection<ProductProjectionForShop> _collection;

    public AddProductsToBasketHandler(IMongoCollection<ProductProjectionForShop> collection)
    {
        _collection = collection;
    }

    public async Task<IResult> HandleAsync(GetPricesForShop command, CancellationToken cancellationToken)
    {
        var (productId, shopChainId) = command.Body;
        var streamId = new ProductStreamId(productId, shopChainId);
        var result = await _collection
            .Find(x => x.Id == streamId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return result is null ? Results.NotFound() : Results.Ok(result);
    }
}




using IGroceryStore.Baskets.Projectors;
using IGroceryStore.Baskets.ValueObjects;
using IGroceryStore.Shared.Abstraction;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;

namespace IGroceryStore.Baskets.Features.Products;

internal record GetPricesForShop(ulong ProductId, ulong ShopId) : IHttpQuery;

public class GetPricesForShopEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Baskets.MapGet<GetPricesForShop>("{shopId}/{productId}")
            .Produces<ProductProjectionForShop>();
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
        var (productId, shopChainId) = command;
        var streamId = new ProductStreamId(productId, shopChainId);
        var result = await _collection
            .Find(x => x.Id == streamId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return result is null ? Results.NotFound() : Results.Ok(result);
    }
}




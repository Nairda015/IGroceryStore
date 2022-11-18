using IGroceryStore.Products.Common;
using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Products.ReadModels;
using IGroceryStore.Shared.EndpointBuilders;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Products.Queries;

internal record GetProducts(uint PageNumber, uint PageSize, ulong CategoryId) 
    : QueryForPaginatedResult(PageNumber, PageSize), IHttpQuery;

public class GetProductsEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) => 
        builder.Products.MapGet<GetProducts, GetProductsHandler>("");
}

internal class GetProductsHandler : IHttpQueryHandler<GetProducts>
{
    private readonly ProductsDbContext _productsDbContext;

    public GetProductsHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(GetProducts query, CancellationToken cancellationToken = default)
    {
        var (pageNumber, pageSize, categoryId) = query;
        var products = _productsDbContext.Products
            .Where(x => x.CategoryId == categoryId)
            .Select(x => new ProductReadModel
            {
                Id = x.Id,
                Name = x.Name,
                BrandName = x.Brand.Name,
                Quantity = new QuantityReadModel(x.Quantity.Amount, x.Quantity.Unit.Name)
            })
            .AsNoTracking()
            .AsQueryable();

        var result = await PaginatedList<ProductReadModel>
            .CreateAsync(products, pageNumber, pageSize);
        return Results.Ok(result);
    }
}

using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Queries;
using IGroceryStore.Shared.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Queries;

internal record GetProducts(uint PageNumber, uint PageSize, ulong CategoryId) 
    : QueryForPaginatedResult(PageNumber, PageSize), IHttpQuery;

public class GetProductsEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) => 
        endpoints.MapGet<GetProducts>("api/products").WithTags(SwaggerTags.Products);
}

internal class GetProductsHandler : IQueryHandler<GetProducts, IResult>
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

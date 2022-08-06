using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Queries;
using IGroceryStore.Shared.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Queries;

internal record GetProducts(uint PageNumber, uint PageSize, CategoryId CategoryId) 
    : QueryForPaginatedResult(PageNumber, PageSize), IQuery<PaginatedList<ProductReadModel>>;

public class GetProductsEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("products/{pageNumber:int}/{pageSize:int}/{categoryId}",
            async (IQueryDispatcher dispatcher,
                    [AsParameters] GetProducts query,
                    CancellationToken cancellationToken) =>
                Results.Ok(await dispatcher.QueryAsync(query, cancellationToken)))
            .WithTags(SwaggerTags.Products);
    }
}

internal class GetProductsHandler : IQueryHandler<GetProducts, PaginatedList<ProductReadModel>>
{
    private readonly ProductsDbContext _productsDbContext;

    public GetProductsHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<PaginatedList<ProductReadModel>> HandleAsync(GetProducts query, CancellationToken cancellationToken = default)
    {
        var (pageNumber, pageSize, categoryId) = query;
        var products = _productsDbContext.Products
            .Where(x => x.CategoryId == categoryId)
            .Select(x => new ProductReadModel()
            {
                Id = x.Id,
                Name = x.Name,
                BrandName = x.Brand.Name,
                Quantity = new QuantityReadModel(x.Quantity.Amount, x.Quantity.Unit.Name)
            })
            .AsNoTracking()
            .AsQueryable();

        return await PaginatedList<ProductReadModel>.CreateAsync(products, pageNumber, pageSize);
    }
}
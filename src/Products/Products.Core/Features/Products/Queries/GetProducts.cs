using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Queries;
using IGroceryStore.Shared.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Queries;

internal record GetProducts(ulong CategoryId, int PageNumber, int PageSize) : IQuery<PaginatedList<ProductReadModel>>;

public class GetProductsEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("products/{pageNumber:int}/{pageSize:int}/{categoryId}",
            async (IQueryDispatcher dispatcher,
                    [FromRoute] int pageNumber,
                    [FromRoute] int pageSize,
                    [FromRoute] ulong categoryId,
                    CancellationToken cancellationToken) =>
                Results.Ok(await dispatcher.QueryAsync(new GetProducts(categoryId, pageNumber, pageSize), cancellationToken)));
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
        var (categoryId, pageNumber, pageSize) = query;
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
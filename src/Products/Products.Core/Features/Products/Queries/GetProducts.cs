using IGroceryStore.Products.Contracts.ReadModels;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Queries;
using IGroceryStore.Shared.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Queries;

internal record GetProducts(ulong CategoryId, int PageNumber, int PageSize) : IQuery<PaginatedList<ProductReadModel>>;

public class GetProductsController : ProductsControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public GetProductsController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("products/{pageNumber:int}/{pageSize:int}/{categoryId}")]
    public async Task<ActionResult<PaginatedList<ProductDetailsReadModel>>> GetProducts([FromRoute] int pageNumber, [FromRoute] int pageSize, [FromRoute] ulong categoryId, CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.QueryAsync(new GetProducts(categoryId, pageNumber, pageSize), cancellationToken);
        return Ok(result);
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
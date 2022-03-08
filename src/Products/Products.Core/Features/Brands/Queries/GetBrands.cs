using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Brands.Queries;
public record GetBrandsResult(List<BrandReadModel> Brands);
internal record GetBrands : IQuery<GetBrandsResult>;

public class GetBrandsController : ProductsControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public GetBrandsController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("brands")]
    public async Task<ActionResult<GetBrandsResult>> GetBrands(CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.QueryAsync(new GetBrands(), cancellationToken);
        return Ok(result);
    }
}

internal class GetBrandsHandler : IQueryHandler<GetBrands, GetBrandsResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public GetBrandsHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<GetBrandsResult> HandleAsync(GetBrands query, CancellationToken cancellationToken = default)
    {
        var brands = await _productsDbContext.Brands
            .Select(c => new BrandReadModel(c.Id, c.Name))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new GetBrandsResult(brands);
    }
}
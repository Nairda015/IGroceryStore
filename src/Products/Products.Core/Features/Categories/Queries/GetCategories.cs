using IGroceryStore.Products.Contracts.ReadModels;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Categories.Queries;
public record GetCategoriesResult(List<CategoryReadModel> Categories);
internal record GetCategories : IQuery<GetCategoriesResult>;


public class GetCategoriesController : ProductsControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public GetCategoriesController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("categories")]
    public async Task<ActionResult<GetCategoriesResult>> GetCategories(CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.QueryAsync(new GetCategories(), cancellationToken);
        return Ok(result);
    }
}

internal class GetCategoriesHandler : IQueryHandler<GetCategories, GetCategoriesResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public GetCategoriesHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<GetCategoriesResult> HandleAsync(GetCategories query, CancellationToken cancellationToken = default)
    {
        var categories = await _productsDbContext.Categories
            .Select(c => new CategoryReadModel(c.Id, c.Name))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new GetCategoriesResult(categories);
    }
}
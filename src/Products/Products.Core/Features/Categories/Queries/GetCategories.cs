using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Categories.Queries;
public record GetCategoriesResult(List<CategoryReadModel> Categories);
internal record GetCategories : IQuery<GetCategoriesResult>;


public class GetCategoriesEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("categories",
            async (IQueryDispatcher dispatcher, CancellationToken cancellationToken) =>
                Results.Ok(await dispatcher.QueryAsync(new GetCategories(), cancellationToken)))
            .WithTags(SwaggerTags.Products);
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
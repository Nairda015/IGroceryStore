using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Categories.Queries;

internal record GetCategories : IHttpQuery;

public class GetCategoriesEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet<GetCategories>("categories").WithTags(SwaggerTags.Products);
}

internal class GetCategoriesHandler : IQueryHandler<GetCategories, IResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public GetCategoriesHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(GetCategories query, CancellationToken cancellationToken = default)
    {
        var categories = await _productsDbContext.Categories
            .Select(c => new CategoryReadModel(c.Id, c.Name))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new GetCategoriesResult(categories);
        return Results.Ok(result);
    }
    
    private record GetCategoriesResult(List<CategoryReadModel> Categories);

}
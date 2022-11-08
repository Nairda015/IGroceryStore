using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Products.ReadModels;
using IGroceryStore.Shared.Abstraction;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Categories.Queries;

internal record GetCategories : IHttpQuery;
internal record GetCategoriesResult(List<CategoryReadModel> Categories);


public class GetCategoriesEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet<GetCategories>("api/categories")
            .Produces<GetCategoriesResult>()
            .WithTags(Constants.SwaggerTags.Products);
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
}

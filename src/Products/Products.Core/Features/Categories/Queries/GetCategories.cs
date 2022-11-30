using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Products.ReadModels;
using IGroceryStore.Shared.EndpointBuilders;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Categories.Queries;

internal record GetCategories : IHttpQuery;
internal record GetCategoriesResult(List<CategoryReadModel> Categories);


public class GetCategoriesEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Products.MapGet<GetCategories, GetCategoriesHttpHandler>("categories")
            .Produces<GetCategoriesResult>();
}

internal class GetCategoriesHttpHandler : IHttpQueryHandler<GetCategories>
{
    private readonly ProductsDbContext _productsDbContext;

    public GetCategoriesHttpHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(GetCategories query, CancellationToken cancellationToken)
    {
        var categories = await _productsDbContext.Categories
            .Select(c => new CategoryReadModel(c.Id, c.Name))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new GetCategoriesResult(categories);
        return Results.Ok(result);
    }
}

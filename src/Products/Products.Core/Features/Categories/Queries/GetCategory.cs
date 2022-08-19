using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Categories.Queries;

internal record GetCategory(ulong Id) : IHttpQuery;

public class GetCategoryEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet<GetCategory>("categories/{id}")
            .Produces(404)
            .Produces(200)
            .WithTags(SwaggerTags.Products);
}

internal class GetCategoryHandler : IQueryHandler<GetCategory, IResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public GetCategoryHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(GetCategory query, CancellationToken cancellationToken = default)
    {
        var category = 
            await _productsDbContext.Categories.FirstOrDefaultAsync(x => x.Id.Equals(query.Id), cancellationToken);

        if (category is null) throw new CategoryNotFoundException(query.Id);

        var result = new CategoryReadModel(category.Id, category.Name);
        return Results.Ok(result);
    }
} 
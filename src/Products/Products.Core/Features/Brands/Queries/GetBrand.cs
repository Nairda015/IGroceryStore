using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Products.ReadModels;
using IGroceryStore.Shared.Abstraction;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Brands.Queries;

internal record GetBrand(ulong id) : IHttpQuery;

public class GetBrandEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet<GetBrand>("api/brands/{id}")
            .Produces<BrandReadModel>()
            .Produces(404)
            .WithName(nameof(GetBrand))
            .WithTags(Constants.SwaggerTags.Products);
}
internal class GetBrandHandler : IQueryHandler<GetBrand, IResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public GetBrandHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(GetBrand query, CancellationToken cancellationToken = default)
    {
        var brand = await _productsDbContext.Brands
            .FirstOrDefaultAsync(x => x.Id == query.id, cancellationToken);

        if (brand is null) return Results.NotFound();

        var result = new BrandReadModel(brand.Id, brand.Name);
        return Results.Ok(result);
    }
}

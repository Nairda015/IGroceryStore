using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Products.ReadModels;
using IGroceryStore.Shared.EndpointBuilders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Brands.Queries;

internal record GetBrand(ulong id) : IHttpQuery;

public class GetBrandEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Products.MapGet<GetBrand, GetBrandHttpHandler>("brands/{id}")
            .Produces<BrandReadModel>()
            .Produces(404)
            .WithName(nameof(GetBrand));
}
internal class GetBrandHttpHandler : IHttpQueryHandler<GetBrand>
{
    private readonly ProductsDbContext _productsDbContext;

    public GetBrandHttpHandler(ProductsDbContext productsDbContext)
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

using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Products.ReadModels;
using IGroceryStore.Shared.EndpointBuilders;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Brands.Queries;

internal record GetBrands : IHttpQuery;

public class GetBrandsEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Products.MapGet<GetBrands, GetBrandsHttpHandler>("brands");
}

internal class GetBrandsHttpHandler : IHttpQueryHandler<GetBrands>
{
    private readonly ProductsDbContext _productsDbContext;

    public GetBrandsHttpHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(GetBrands query, CancellationToken cancellationToken)
    {
        var brands = await _productsDbContext.Brands
            .Select(c => new BrandReadModel(c.Id, c.Name))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new GetBrandsResult(brands);
        return Results.Ok(brands);
    }
    private record GetBrandsResult(List<BrandReadModel> Brands);
}

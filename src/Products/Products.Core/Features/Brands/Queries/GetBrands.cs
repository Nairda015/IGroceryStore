using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Products.ReadModels;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Brands.Queries;

internal record GetBrands : IHttpQuery;

public class GetBrandsEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet<GetBrands>("api/brands").WithTags(SwaggerTags.Products);
}

internal class GetBrandsHandler : IQueryHandler<GetBrands, IResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public GetBrandsHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(GetBrands query, CancellationToken cancellationToken = default)
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

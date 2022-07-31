using IGroceryStore.Products.Contracts.ReadModels;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Brands.Queries;
public record GetBrandsResult(List<BrandReadModel> Brands);
internal record GetBrands : IQuery<GetBrandsResult>;

public class GetBrandsEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("brands",
            async (IQueryDispatcher dispatcher, CancellationToken cancellationToken) =>
                Results.Ok(await dispatcher.QueryAsync(new GetBrands(), cancellationToken)));
    }
}

internal class GetBrandsHandler : IQueryHandler<GetBrands, GetBrandsResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public GetBrandsHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<GetBrandsResult> HandleAsync(GetBrands query, CancellationToken cancellationToken = default)
    {
        var brands = await _productsDbContext.Brands
            .Select(c => new BrandReadModel(c.Id, c.Name))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new GetBrandsResult(brands);
    }
}
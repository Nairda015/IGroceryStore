using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Countries.Queries;

internal record GetCountries : IHttpQuery;

public class GetCountriesEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet<GetCountries>("countries").WithTags(SwaggerTags.Products);
}

internal class GetCountriesHandler : IQueryHandler<GetCountries, IResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public GetCountriesHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(GetCountries query, CancellationToken cancellationToken = default)
    {
        var countries = await _productsDbContext.Countries
            .Select(c => new CountryReadModel(c.Id, c.Name, c.Code))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new GetCountriesResult(countries);
        return Results.Ok(result);
    }
    
    private record GetCountriesResult(IEnumerable<CountryReadModel> Countries);
}
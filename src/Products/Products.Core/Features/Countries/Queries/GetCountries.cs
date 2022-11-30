using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Products.ReadModels;
using IGroceryStore.Shared.EndpointBuilders;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Countries.Queries;

internal record GetCountries : IHttpQuery;

public class GetCountriesEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Products.MapGet<GetCountries, GetCountriesHttpHandler>("countries");
}

internal class GetCountriesHttpHandler : IHttpQueryHandler<GetCountries>
{
    private readonly ProductsDbContext _productsDbContext;

    public GetCountriesHttpHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(GetCountries query, CancellationToken cancellationToken)
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

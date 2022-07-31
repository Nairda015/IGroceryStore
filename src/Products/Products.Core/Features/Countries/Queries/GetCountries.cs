using IGroceryStore.Products.Contracts.ReadModels;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Countries.Queries;

public record GetCountriesResult(IEnumerable<CountryReadModel> Countries);
internal record GetCountries : IQuery<GetCountriesResult>;

public class GetCountriesEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("countries",
            async (IQueryDispatcher dispatcher, CancellationToken cancellationToken) =>
                Results.Ok(await dispatcher.QueryAsync(new GetCountries(), cancellationToken)));
    }
}

internal class GetCountriesHandler : IQueryHandler<GetCountries, GetCountriesResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public GetCountriesHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<GetCountriesResult> HandleAsync(GetCountries query, CancellationToken cancellationToken = default)
    {
        var countries = await _productsDbContext.Countries
            .Select(c => new CountryReadModel(c.Id, c.Name, c.Code))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new GetCountriesResult(countries);
    }
}
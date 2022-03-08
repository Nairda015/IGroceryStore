using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Countries.Queries;

public record GetCountriesResult(IEnumerable<CountryReadModel> Countries);
internal record GetCountries : IQuery<GetCountriesResult>;

public class GetCountriesController : ProductsControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public GetCountriesController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("countries")]
    public async Task<ActionResult<GetCountriesResult>> GetCountries(CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.QueryAsync(new GetCountries(), cancellationToken);
        return Ok(result);
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
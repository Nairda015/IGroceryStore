using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Shared.Abstraction.Queries;
using IGroceryStore.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Queries;

public record GetProduct(Guid Id) : IQuery<ProductDetailsReadModel>;

public class GetProductController : ApiControllerBase
{
    private readonly IQueryDispatcher _dispatcher;

    public GetProductController(IQueryDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDetailsReadModel>> GetProduct([FromRoute] Guid id)
    {
        var result = await _dispatcher.QueryAsync(new GetProduct(id));
        return Ok(result);
    }
}

internal class GetProductHandler : IQueryHandler<GetProduct, ProductDetailsReadModel>
{
    private readonly ProductsDbContext _context;

    public GetProductHandler(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDetailsReadModel> HandleAsync(GetProduct query, CancellationToken cancellationToken = default)
    {
        var result = await _context.Products
            .AsNoTracking()
            .Select(x => new ProductDetailsReadModel()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                BarCode = x.BarCode,
                Quantity = new QuantityReadModel(x.Quantity.Amount, x.Quantity.Unit.Name),
                IsObsolete = x.IsObsolete,
                CountryName = x.Country.Name,
                BrandName = x.Brand.Name,
                CategoryName = x.Category.Name,
                Allergens = x.Allergens
                    .Select(a => new AllergenReadModel(a.Name, a.Code))
            })
            .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);

        if (result is null) throw new ProductNotFoundException(query.Id);
        return result;
    }
}
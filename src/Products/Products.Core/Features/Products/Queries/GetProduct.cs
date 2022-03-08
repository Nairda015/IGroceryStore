using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Queries;

public record GetProduct(Guid Id) : IQuery<ProductDetailsReadModel>;

public class GetProductController : ProductsControllerBase
{
    private readonly IQueryDispatcher _dispatcher;

    public GetProductController(IQueryDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet("products/{id:guid}")]
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
        var model = await _context.Products
            .Include(x => x.Category)
            .Include(x => x.Brand)
            .Include(x => x.Country)
            .AsNoTracking()
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Description,
                x.BarCode,
                Quantity = new QuantityReadModel(x.Quantity.Amount, x.Quantity.Unit.Name),
                x.IsObsolete,
                CountryName = x.Country.Name,
                BrandName = x.Brand.Name,
                CategoryName = x.Category.Name,
                x.Allergens
                
            })
            .FirstOrDefaultAsync(x => x.Id.Equals(query.Id), cancellationToken);
     
        if (model is null) throw new ProductNotFoundException(query.Id);
        var result = new ProductDetailsReadModel()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            BarCode = model.BarCode,
            Quantity = model.Quantity,
            IsObsolete = model.IsObsolete,
            CountryName = model.CountryName,
            BrandName = model.BrandName,
            CategoryName = model.CategoryName,
        };
        if (model.Allergens is null) return result;

        result.Allergens = model.Allergens.Select(x => new AllergenReadModel(x.Name, x.Code)); 
        return result;
    }
}
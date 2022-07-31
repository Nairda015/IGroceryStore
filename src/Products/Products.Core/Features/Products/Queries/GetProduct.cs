using IGroceryStore.Products.Contracts.ReadModels;
using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Queries;

public record GetProduct(ulong Id) : IQuery<ProductDetailsReadModel>;

public class GetProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("products/{id}",
            async (IQueryDispatcher dispatcher, ulong id, CancellationToken cancellationToken) =>
                Results.Ok(await dispatcher.QueryAsync(new GetProduct(id), cancellationToken)));
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
        if (model.Allergens.Any()) return result;

        result.Allergens = model.Allergens.Select(x => new AllergenReadModel(x.Id, x.Name)); 
        return result;
    }
}
﻿using IGroceryStore.Products.Exceptions;
using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Products.ReadModels;
using IGroceryStore.Shared.EndpointBuilders;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Products.Queries;

internal record GetProduct(ulong Id) : IHttpQuery;

public class GetProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Products.MapGet<GetProduct, GetProductHandler>("{id}");
}

internal class GetProductHandler : IHttpQueryHandler<GetProduct>
{
    private readonly ProductsDbContext _context;

    public GetProductHandler(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> HandleAsync(GetProduct query, CancellationToken cancellationToken)
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
        var result = new ProductDetailsReadModel
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
        if (model.Allergens.Any()) return Results.Ok(result);

        result.Allergens = model.Allergens.Select(x => new AllergenReadModel(x.Id, x.Name)); 
        return Results.Ok(result);
    }
}

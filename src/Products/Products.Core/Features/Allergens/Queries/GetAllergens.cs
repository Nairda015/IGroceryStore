﻿using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Products.ReadModels;
using IGroceryStore.Shared.Abstraction;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Allergens.Queries;

internal record GetAllergens : IHttpQuery;

public class GetAllergensEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Products.MapGet<GetAllergens>("allergens");
}

internal class GetAllergensHandler : IQueryHandler<GetAllergens, IResult>
{
    private readonly ProductsDbContext _context;

    public GetAllergensHandler(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> HandleAsync(GetAllergens query,
        CancellationToken cancellationToken = default)
    {
        var allergens =  await _context.Allergens
            .Select(x => new AllergenReadModel(x.Id, x.Name))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new GetAllergensResult(allergens);
        return Results.Ok(result);
    }
    
    private record GetAllergensResult(IEnumerable<AllergenReadModel> Allergens);
}

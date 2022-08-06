﻿using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Commands;

public record AddAllergenToProduct(ulong Id, ulong AllergenId) : ICommand;

public class AddAllergenToProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("products/add-allergen",
            async (ICommandDispatcher dispatcher,
                AddAllergenToProduct command,
                CancellationToken cancellationToken) =>
            {
                await dispatcher.DispatchAsync(command, cancellationToken);
                return Results.Accepted();
            }).WithTags(SwaggerTags.Products);
    }
}

internal class AddAllergenToProductHandler : ICommandHandler<AddAllergenToProduct>
{
    private readonly ProductsDbContext _productsDbContext;

    public AddAllergenToProductHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task HandleAsync(AddAllergenToProduct command, CancellationToken cancellationToken = default)
    {
        var (productId, allergenId) = command;
        var product =
            await _productsDbContext.Products.FirstOrDefaultAsync(x => x.Id.Equals(productId), cancellationToken);
        if (product == null) throw new ProductNotFoundException(productId);

        var allergen =
            await _productsDbContext.Allergens.FirstOrDefaultAsync(x => x.Id == allergenId, cancellationToken);
        if (allergen == null) throw new AllergenNotFoundException(allergenId);


        product.AddAllergen(allergen);
        _productsDbContext.Update(product);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
    }
}
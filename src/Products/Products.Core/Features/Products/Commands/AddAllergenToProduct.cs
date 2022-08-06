using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Commands;

public record AddAllergenToProduct(ulong Id, ulong AllergenId) : IHttpCommand;

public class AddAllergenToProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut<AddAllergenToProduct>("products/add-allergen").WithTags(SwaggerTags.Products);
    }
}

internal class AddAllergenToProductHandler : ICommandHandler<AddAllergenToProduct, IResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public AddAllergenToProductHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(AddAllergenToProduct command, CancellationToken cancellationToken = default)
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
        
        return Results.Ok();
    }
}
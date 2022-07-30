using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Commands;

public record AddAllergenToProduct(ulong Id, ulong AllergenId) : ICommand;

public class AddAllergenToProductController : ProductsControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public AddAllergenToProductController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost("products/add-allergen")]
    public async Task<ActionResult> AddAllergenToProduct([FromBody] AddAllergenToProduct command, CancellationToken cancellationToken)
    {
        await _commandDispatcher.DispatchAsync(command, cancellationToken);
        return Ok();
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
        var product = await _productsDbContext.Products.FirstOrDefaultAsync(x => x.Id.Equals(productId), cancellationToken);
        if (product == null)throw new ProductNotFoundException(productId);

        var allergen = await _productsDbContext.Allergens.FirstOrDefaultAsync(x => x.Id == allergenId, cancellationToken);
        if (allergen == null) throw new AllergenNotFoundException(allergenId);
        
        
        product.AddAllergen(allergen);
        _productsDbContext.Update(product);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
    }
}
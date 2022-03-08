using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Commands;

public record MarkAsObsolete(ProductId Id) : ICommand;

public class MarkAsObsoleteController : ProductsControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public MarkAsObsoleteController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost("products/mark-as-obsolete/{id:guid}")]
    public async Task<ActionResult> MarkAsObsolete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _commandDispatcher.DispatchAsync(new MarkAsObsolete(id), cancellationToken);
        return Ok();
    }
}

internal class MarkAsObsoleteHandler : ICommandHandler<MarkAsObsolete>
{
    private readonly ProductsDbContext _productsDbContext;

    public MarkAsObsoleteHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task HandleAsync(MarkAsObsolete command, CancellationToken cancellationToken = default)
    {
        var product = await _productsDbContext.Products.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
        if (product == null) throw new ProductNotFoundException(command.Id);

        product.MarkAsObsolete();
        _productsDbContext.Update(product);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
    }
}
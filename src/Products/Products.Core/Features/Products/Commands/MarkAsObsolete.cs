using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Commands;

public record MarkAsObsolete(ProductId Id) : ICommand;

public class MarkAsObsoleteEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("products/mark-as-obsolete/{id}",
            async (ICommandDispatcher dispatcher,
                ulong id,
                CancellationToken cancellationToken) =>
            {
                await dispatcher.DispatchAsync(new MarkAsObsolete(id), cancellationToken);
                return Results.Accepted();
            });
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
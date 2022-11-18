using IGroceryStore.Products.Exceptions;
using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Shared.EndpointBuilders;
using IGroceryStore.Shared.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Products.Commands;

internal record MarkAsObsolete(MarkAsObsolete.MarkAsObsoleteBody Body) : IHttpCommand
{
    internal record MarkAsObsoleteBody(ProductId Id);
}

public class MarkAsObsoleteEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Products.MapPost<MarkAsObsolete, MarkAsObsoleteHandler>("mark-as-obsolete/{id}");
}

internal class MarkAsObsoleteHandler : IHttpCommandHandler<MarkAsObsolete>
{
    private readonly ProductsDbContext _productsDbContext;

    public MarkAsObsoleteHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(MarkAsObsolete command, CancellationToken cancellationToken = default)
    {
        var product = await _productsDbContext.Products.FirstOrDefaultAsync(x => x.Id == command.Body.Id, cancellationToken);
        if (product == null) throw new ProductNotFoundException(command.Body.Id);

        product.MarkAsObsolete();
        _productsDbContext.Update(product);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }
}

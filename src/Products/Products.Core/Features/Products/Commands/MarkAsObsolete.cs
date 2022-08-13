using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Commands;

internal record MarkAsObsolete(ProductId Id);
internal record MarkAsObsoleteCommand(MarkAsObsolete Value) : IHttpCommand;

public class MarkAsObsoleteEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<MarkAsObsoleteCommand>("products/mark-as-obsolete/{id}").WithTags(SwaggerTags.Products);
}

internal class MarkAsObsoleteHandler : ICommandHandler<MarkAsObsoleteCommand, IResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public MarkAsObsoleteHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(MarkAsObsoleteCommand command, CancellationToken cancellationToken = default)
    {
        var product = await _productsDbContext.Products.FirstOrDefaultAsync(x => x.Id == command.Value.Id, cancellationToken);
        if (product == null) throw new ProductNotFoundException(command.Value.Id);

        product.MarkAsObsolete();
        _productsDbContext.Update(product);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }
}
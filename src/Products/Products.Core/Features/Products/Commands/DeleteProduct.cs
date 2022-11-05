using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Commands;

internal record DeleteProduct(ulong Id) : IHttpCommand;

public class DeleteProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapDelete<DeleteProduct>("products/{id}")
            //.RequireAuthorization()
            .WithTags(SwaggerTags.Products)
            .Produces(204)
            .Produces(400);
            
}

internal class DeleteProductHandler : ICommandHandler<DeleteProduct, IResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public DeleteProductHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(DeleteProduct command, CancellationToken cancellationToken = default)
    {
        var products = await _productsDbContext.Products.ToListAsync();

        var product = 
            await _productsDbContext.Products.FirstOrDefaultAsync(x => x.Id.Equals(command.Id), cancellationToken);

        if (product is null) throw new ProductNotFoundException(command.Id);

        _productsDbContext.Products.Remove(product);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        
        return Results.NoContent();
    }
}

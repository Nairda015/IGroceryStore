using IGroceryStore.Products.Exceptions;
using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Shared.EndpointBuilders;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Categories.Commands;

internal record DeleteCategory(ulong Id) : IHttpCommand;

public class DeleteCategoryEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Products.MapDelete<DeleteCategory, DeleteCategoryHandler>("categories/{id}")
            .Produces(204)
            .Produces(400);
}

internal class DeleteCategoryHandler : IHttpCommandHandler<DeleteCategory>
{
    private readonly ProductsDbContext _productsDbContext;

    public DeleteCategoryHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(DeleteCategory command, CancellationToken cancellationToken = default)
    {
        var category =
            await _productsDbContext.Categories.FirstOrDefaultAsync(x => x.Id.Equals(command.Id), cancellationToken);

        if (category is null) throw new CategoryNotFoundException(command.Id);

        var isAnyReference = 
            await _productsDbContext.Products.AnyAsync(x => x.CategoryId.Equals(command.Id), cancellationToken);

        if (isAnyReference) throw new CategoryHasReferenceException(command.Id); 

        _productsDbContext.Categories.Remove(category);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}

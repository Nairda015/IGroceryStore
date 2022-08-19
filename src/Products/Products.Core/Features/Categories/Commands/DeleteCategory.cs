using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Categories.Commands;

internal record DeleteCategory(ulong Id) : IHttpCommand;

public class DeleteCategoryEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapDelete<DeleteCategory>("categories/{id}")
            .WithTags(SwaggerTags.Products)
            .Produces(204)
            .Produces(400);
}

internal class DeleteCategoryHandler : ICommandHandler<DeleteCategory, IResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public DeleteCategoryHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(DeleteCategory command, CancellationToken cancellationToken = default)
    {
        var isAnyReference = 
            await _productsDbContext.Products.AnyAsync(x => x.CategoryId.Equals(command.Id), cancellationToken);

        if (isAnyReference) throw new CategoryHasReferenceException(command.Id); 

        var category =
            await _productsDbContext.Categories.FirstOrDefaultAsync(x => x.Id.Equals(command.Id), cancellationToken);

        if (category is null) throw new CategoryNotFoundException(command.Id);

        _productsDbContext.Categories.Remove(category);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}
using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Categories.Commands;

internal record UpdateCategory(UpdateCategory.UpdateCategoryBody Body, ulong Id) : IHttpCommand
{
    internal record UpdateCategoryBody(ulong CategoryId, string Name);
}

public class UpdateCategoryEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPut<UpdateCategory>("category/{id}")
            .WithTags(SwaggerTags.Products)
            .Produces(200)
            .Produces(400);
}

internal class UpdateCategoryHandler : ICommandHandler<UpdateCategory, IResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public UpdateCategoryHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(UpdateCategory command, CancellationToken cancellationToken = default)
    {
        var (categoryId, name) = command.Body;
        var category =
            await _productsDbContext.Categories.FirstOrDefaultAsync(x => x.Id.Equals(categoryId), cancellationToken);

        if (category is null) throw new CategoryNotFoundException(categoryId);

        _productsDbContext.Update(category);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }
}
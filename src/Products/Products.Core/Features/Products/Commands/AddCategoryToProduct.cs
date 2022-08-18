using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Commands;

internal record AddCategoryToProduct(AddCategoryToProduct.AddCategoryToProductBody Body) : IHttpCommand
{
    internal record AddCategoryToProductBody(ulong Id, ulong CategoryId);
}

public class AddCategoryToProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPut<AddCategoryToProduct>("products/add-category")
            .Produces(200)
            .Produces(400)
            .WithTags(SwaggerTags.Products);
}

internal class AddCategoryToProductHandler : ICommandHandler<AddCategoryToProduct, IResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public AddCategoryToProductHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(AddCategoryToProduct command, CancellationToken cancellationToken = default)
    {
        var (productId, categoryId) = command.Body;
        var product =
            await _productsDbContext.Products.FirstOrDefaultAsync(x => x.Id.Equals(productId), cancellationToken);
        if (product == null) throw new ProductNotFoundException(productId);

        var category =
            await _productsDbContext.Categories.FirstOrDefaultAsync(x => x.Id == categoryId, cancellationToken);
        if (category == null) throw new CategoryNotFoundException(categoryId);


        product.Category = category;
        _productsDbContext.Update(product);
        await _productsDbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}
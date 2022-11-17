using IGroceryStore.Products.Exceptions;
using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Brands.Commands;

internal record DeleteBrand(ulong Id) : IHttpCommand;

public class DeleteBrandEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapDelete<DeleteBrand>("api/brands/{id}")
            .WithTags(Constants.SwaggerTags.Products)
            .Produces(204)
            .Produces(400)
            .Produces(404);

}
internal class DeleteBrandHandler : ICommandHandler<DeleteBrand, IResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public DeleteBrandHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(DeleteBrand command, CancellationToken cancellationToken = default)
    {
        var brand =
            await _productsDbContext.Brands.FirstOrDefaultAsync(x => x.Id.Equals(command.Id), cancellationToken);

        if (brand is null) return Results.NotFound();

        var isAnyReference =
            await _productsDbContext.Products.AnyAsync(x => x.BrandId.Equals(command.Id), cancellationToken);

        if (isAnyReference) return Results.BadRequest();

        _productsDbContext.Brands.Remove(brand);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}


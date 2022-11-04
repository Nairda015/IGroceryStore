using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Brands.Commands;

internal record UpdateBrand(UpdateBrand.UpdateBrandBody Body, ulong Id) : IHttpCommand
{
    internal record UpdateBrandBody(string Name);
}

public class UpdateBrandEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPut<UpdateBrand>("api/brands/{id}")
            .WithTags(SwaggerTags.Products)
            .Produces(202)
            .Produces(404);
}

internal class UpdateBrandHandler : ICommandHandler<UpdateBrand, IResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public UpdateBrandHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(UpdateBrand command, CancellationToken cancellationToken = default)
    {
        var brand =
            await _productsDbContext.Brands
                .FirstOrDefaultAsync(x => x.Id.Equals(command.Id), cancellationToken);

        if (brand is null) return Results.NotFound();

        brand.Name = command.Body.Name;
        _productsDbContext.Update(brand);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return Results.Accepted();
    }
}

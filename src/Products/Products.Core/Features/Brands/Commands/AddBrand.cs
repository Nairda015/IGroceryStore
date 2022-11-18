using IGroceryStore.Products.Entities;
using IGroceryStore.Products.Features.Brands.Queries;
using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Shared.EndpointBuilders;
using IGroceryStore.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Brands.Commands;

internal record AddBrand(AddBrand.AddBrandBody Body) : IHttpCommand
{
    internal record AddBrandBody(string Name);
}

public class AddBrandEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
    builder.Products.MapPost<AddBrand, AddBrandHandler>("brands")
        .Produces(201)
        .Produces(400);
    
}

internal class AddBrandHandler : IHttpCommandHandler<AddBrand>
{
    private readonly ProductsDbContext _productsDbContext;
    private readonly ISnowflakeService _snowflakeService;

    public AddBrandHandler(ProductsDbContext productsDbContext, ISnowflakeService snowflakeService)
    {
        _productsDbContext = productsDbContext;
        _snowflakeService = snowflakeService;
    }

    public async Task<IResult> HandleAsync(AddBrand command, CancellationToken cancellationToken = default)
    {
        var alreadyExists = await _productsDbContext.Brands.AnyAsync(b => b.Name.Equals(command.Body.Name), cancellationToken);

        if(alreadyExists)
        {
            return Results.BadRequest();
        }
        

        var brand = new Brand
        {
            Id = _snowflakeService.GenerateId(),
            Name = command.Body.Name
        };

        await _productsDbContext.Brands.AddAsync(brand);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return Results.CreatedAtRoute(nameof(GetBrand),new {brand.Id.Id});
    }
}


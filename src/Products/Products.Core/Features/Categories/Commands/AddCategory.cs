using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Products.Core.Features.Categories.Commands;

internal record AddCategory(AddCategory.AddCategoryBody Body) : IHttpCommand
{
    internal record AddCategoryBody(string Name);
}

public class AddCategoryEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<AddCategory>("category")
            .WithTags(SwaggerTags.Products)
            .Produces(201)
            .Produces(400);
}

internal class AddCategoryHandler : ICommandHandler<AddCategory, IResult>
{
    private readonly ProductsDbContext _productsDbContext;
    private readonly ISnowflakeService _snowFlakeService;

    public AddCategoryHandler(ProductsDbContext productsDbContext, ISnowflakeService snowFlakeService)
    {
        _productsDbContext = productsDbContext;
        _snowFlakeService = snowFlakeService;
    }

    public async Task<IResult> HandleAsync(AddCategory command, CancellationToken cancellationToken = default)
    {
        var category = new Category
        {
            Id = _snowFlakeService.GenerateId(),
            Name = command.Body.Name
        };

        await _productsDbContext.Categories.AddAsync(category, cancellationToken);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok(category.Id);
    }
}
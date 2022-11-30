using FluentValidation;
using IGroceryStore.Products.Entities;
using IGroceryStore.Products.Exceptions;
using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Shared.EndpointBuilders;
using IGroceryStore.Shared.Filters;
using IGroceryStore.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Categories.Commands;

internal record CreateCategory(CreateCategory.CreateCategoryBody Body) : IHttpCommand
{
    internal record CreateCategoryBody(string Name);
}

public class CreateCategoryEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Products.MapPost<CreateCategory, CreateCategoryHandler>("categories")
            .AddEndpointFilter<ValidationFilter<CreateCategory.CreateCategoryBody>>()
            .Produces(201)
            .Produces(400);
}

internal class CreateCategoryHandler : IHttpCommandHandler<CreateCategory>
{
    private readonly ProductsDbContext _productsDbContext;
    private readonly ISnowflakeService _snowFlakeService;

    public CreateCategoryHandler(ProductsDbContext productsDbContext, ISnowflakeService snowFlakeService)
    {
        _productsDbContext = productsDbContext;
        _snowFlakeService = snowFlakeService;
    }

    public async Task<IResult> HandleAsync(CreateCategory command, CancellationToken cancellationToken)
    {
        var isExists = await _productsDbContext.Categories.AnyAsync(x => x.Name.Equals(command.Body.Name), cancellationToken);

        if (isExists) throw new CategoryAlreadyExists(command.Body.Name);

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

internal class CreateCategoryValidator : AbstractValidator<CreateCategory.CreateCategoryBody>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .NotEmpty();
    }
}

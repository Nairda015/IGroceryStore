using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using IGroceryStore.Shared.Validation;

namespace IGroceryStore.Products.Core.Features.Categories.Commands;

internal record UpdateCategory(UpdateCategory.UpdateCategoryBody Body, ulong Id) : IHttpCommand
{
    internal record UpdateCategoryBody(string Name);
}

public class UpdateCategoryEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPut<UpdateCategory>("categories/{id}")
            .AddEndpointFilter<ValidationFilter<UpdateCategory.UpdateCategoryBody>>()
            .WithTags(SwaggerTags.Products)
            .Produces(204)
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
        var category =
            await _productsDbContext.Categories
                .FirstOrDefaultAsync(x => x.Id.Equals(command.Id), cancellationToken);

        if (category is null) throw new CategoryNotFoundException(command.Id);

        category.Name = command.Body.Name;
        _productsDbContext.Update(category);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}

internal class UpdateCategoryValidator : AbstractValidator<UpdateCategory.UpdateCategoryBody>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .NotEmpty();
    }
}
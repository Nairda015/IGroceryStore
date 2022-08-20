using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Products.Contracts.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using IGroceryStore.Shared.Validation;
using MassTransit;

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
            .Produces(202)
            .Produces(400);
}

internal class UpdateCategoryHandler : ICommandHandler<UpdateCategory, IResult>
{
    private readonly ProductsDbContext _productsDbContext;
    private readonly IBus _bus;

    public UpdateCategoryHandler(ProductsDbContext productsDbContext, IBus bus)
    {
        _productsDbContext = productsDbContext;
        _bus = bus;
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
        await _bus.Publish(new CategoryUpdated(category.Id, category.Name), cancellationToken);
        return Results.Accepted();
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
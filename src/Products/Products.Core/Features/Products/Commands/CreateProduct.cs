using FluentValidation;
using IGroceryStore.Products.Contracts.Events;
using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Services;
using IGroceryStore.Shared.Validation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Commands;

internal record CreateProduct(CreateProduct.CreateProductBody Body) : IHttpCommand
{
    internal record CreateProductBody(string Name,
        QuantityReadModel Quantity,
        ulong BrandId,
        ulong CountryId,
        ulong CategoryId,
        string? Description = null);
}

public class CreateProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<CreateProduct>("products")
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<CreateProduct.CreateProductBody>>()
            .WithTags(SwaggerTags.Products);
}

internal class CreateProductHandler : ICommandHandler<CreateProduct, IResult>
{
    private readonly ProductsDbContext _productsDbContext;
    private readonly ISnowflakeService _snowflakeService;
    private readonly IBus _bus;

    public CreateProductHandler(ProductsDbContext productsDbContext, IBus bus, ISnowflakeService snowflakeService)
    {
        _productsDbContext = productsDbContext;
        _bus = bus;
        _snowflakeService = snowflakeService;
    }

    public async Task<IResult> HandleAsync(CreateProduct command, CancellationToken cancellationToken = default)
    {
        var (name, quantityReadModel, brandId, countryId, categoryId, description) = command.Body;
        var categoryName = await _productsDbContext.Categories
            .Where(x => x.Id == categoryId)
            .Select(x => x.Name)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        if (categoryName is null) throw new CategoryNotFoundException(categoryId);

        var quantity = new Quantity(quantityReadModel.Amount, quantityReadModel.Unit);
        var product = new Product
        {
            Id = _snowflakeService.GenerateId(),
            Name = name,
            Description = description,
            Quantity = quantity,
            BrandId = brandId,
            CountryId = countryId,
            CategoryId = categoryId
        };

        await _productsDbContext.Products.AddAsync(product, cancellationToken);
        await _productsDbContext.SaveChangesAsync(cancellationToken);

        var productAddedEvent = new ProductAdded(product.Id, name, categoryName);
        await _bus.Publish(productAddedEvent, cancellationToken);
        return Results.Accepted($"/product/{product.Id}", product.Id);
    }
}

internal class CreateProductValidator : AbstractValidator<CreateProduct.CreateProductBody>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Quantity)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Quantity.Amount)
                    .GreaterThan(0);
                
                RuleFor(x => x.Quantity.Unit)
                    .NotEmpty();
            });
        
        RuleFor(x => x.BrandId)
            .NotEmpty();
        
        RuleFor(x => x.CountryId)
            .NotEmpty();
        
        RuleFor(x => x.CategoryId)
            .NotEmpty();
    }
}
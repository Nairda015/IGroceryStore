using FluentValidation;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Services;
using IGroceryStore.Shared.Validation;
using IGroceryStore.Shops.Contracts.Events;
using IGroceryStore.Shops.Core.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Shops.Core.Features.Products;

internal record ReportCurrentPrice(ReportCurrentPrice.ReportCurrentPriceBody Body) : IHttpCommand
{
    internal record ReportCurrentPriceBody(ulong ShopId, ulong ProductId, decimal Price);
}


public class CreateProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<ReportCurrentPrice>("reportPrice")
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<ReportCurrentPrice.ReportCurrentPriceBody>>()
            .WithTags(SwaggerTags.Shops);
}



internal class CreateProductHandler : ICommandHandler<ReportCurrentPrice, IResult>
{
    private readonly IProductsRepository _productsRepository;
    private readonly ISnowflakeService _snowflakeService;
    private readonly ILogger<CreateProductHandler> _logger;
    private readonly IBus _bus;

    public CreateProductHandler(IProductsRepository productsRepository,
        IBus bus,
        ISnowflakeService snowflakeService,
        ILogger<CreateProductHandler> logger)
    {
        _productsRepository = productsRepository;
        _bus = bus;
        _snowflakeService = snowflakeService;
        _logger = logger;
    }

    public async Task<IResult> HandleAsync(ReportCurrentPrice command, CancellationToken cancellationToken = default)
    {
        //from ui user will click on map and select shop
        var (shopId, productId, price) = command.Body;
        
        //should select only shop from request and lowest price shop
        var product = await _productsRepository.GetAsync(productId, cancellationToken);
        if (product is null)
        {
            _logger.LogError("Product with id {productId} not found", productId);
            return Results.BadRequest("Product not found");
        }
        if (product.DoesPriceChange(shopId, price)) return Results.Ok("Price is the same");

        var isPriceLower = product.IsPriceLower(shopId, price);
        product.UpdatePrice(shopId, price);
        await _productsRepository.UpdateAsync(product, cancellationToken);
        var isPriceLowest = price <= product.LowestPrice;

        var message = new ProductPriceChanged(product.Id, shopId, price, isPriceLowest);
        await _bus.Publish(message, cancellationToken);
        return Results.Accepted($"/product/{product.Id}", product.Id);
    }
}

internal class CreateProductValidator : AbstractValidator<ReportCurrentPrice.ReportCurrentPriceBody>
{
    public CreateProductValidator()
    {
        
        RuleFor(x => x.ProductId)
            .NotEmpty();
        
        RuleFor(x => x.ShopId)
            .NotEmpty();
        
        RuleFor(x => x.Price)
            .NotEmpty()
            .GreaterThan(0);
    }
}

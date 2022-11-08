using FluentValidation;
using IGroceryStore.Shared.Abstraction;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Shared.Services;
using IGroceryStore.Shared.Validation;
using IGroceryStore.Shops.Contracts.Events;
using IGroceryStore.Shops.Entities;
using IGroceryStore.Shops.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Shops.Features.Shops;

internal record AddShopChain(AddShopChain.AddShopChainBody Body) : IHttpCommand
{
    //logo?
    internal record AddShopChainBody(string Name, string FriendlyName);
}

public class CreateProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<AddShopChain>("api/shops/shopChain")
            //.RequireAuthorization() //TODO: admin policy
            .AddEndpointFilter<ValidationFilter<AddShopChain>>()
            .WithTags(Constants.SwaggerTags.Shops);
}

internal class AddShopChainHandler : ICommandHandler<AddShopChain, IResult>
{
    private readonly IShopsRepository _shopsRepository;
    private readonly ISnowflakeService _snowflakeService;
    private readonly ILogger<AddShopChainHandler> _logger;
    private readonly IBus _bus;
    private readonly ICurrentUserService _userService;

    public AddShopChainHandler(IShopsRepository shopsRepository,
        IBus bus,
        ISnowflakeService snowflakeService,
        ILogger<AddShopChainHandler> logger,
        ICurrentUserService userService)
    {
        _shopsRepository = shopsRepository;
        _bus = bus;
        _snowflakeService = snowflakeService;
        _logger = logger;
        _userService = userService;
    }

    public async Task<IResult> HandleAsync(AddShopChain command, CancellationToken cancellationToken)
    {
        var (name, friendlyName) = command.Body;
        // TODO:
        // if (await _shopsRepository.ShopChainExistByNameAsync(name, cancellationToken)) 
        //     return Results.BadRequest("Shop chain with this name already exists");
        
        var shopChain = new ShopChain
        {
            Id = _snowflakeService.GenerateId(), Name = name, FriendlyName = friendlyName
        };
        await _shopsRepository.AddChainAsync(shopChain, cancellationToken);
        
        var message = new ShopChainAdded(shopChain.Id, name, friendlyName);
        await _bus.Publish(message, cancellationToken);
        _logger.LogInformation("Shop chain added {shopId} by {userId}", shopChain.Id, _userService.UserId);
        return Results.AcceptedAtRoute($"/shops/shopChain/{shopChain.Id}", shopChain.Id); //TODO: add location
    }
}

internal class CreateProductValidator : AbstractValidator<AddShopChain>
{
    public CreateProductValidator()
    {

        RuleFor(x => x.Body.Name)
            .NotEmpty()
            .MaximumLength(50);
        
        RuleFor(x => x.Body.FriendlyName)
            .NotEmpty()
            .MaximumLength(50);
    }
}

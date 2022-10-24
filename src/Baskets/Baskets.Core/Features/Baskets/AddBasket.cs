using IGroceryStore.Baskets.Core.Entities;
using IGroceryStore.Baskets.Core.Events;
using IGroceryStore.Baskets.Core.Factories;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using Microsoft.AspNetCore.Routing;
using IGroceryStore.Shared.Abstraction.Constants;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Baskets.Core.Features.Baskets;

internal record AddBasket(AddBasket.AddBasketBody Body) : IHttpCommand
{
    public record AddBasketBody(string Name);
}

public class AddBasketEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<AddBasket>("/basket")
            .Produces<Guid>()
            .WithTags(SwaggerTags.Baskets);
}

internal class AddBasketHandler : ICommandHandler<AddBasket, IResult>
{
    private readonly IBasketFactory _factory;
    private readonly IDocumentSession _session;
    private readonly ILogger<AddBasketHandler> _logger;

    public AddBasketHandler(IBasketFactory factory, IDocumentSession session, ILogger<AddBasketHandler> logger)
    {
        _factory = factory;
        _session = session;
        _logger = logger;
    }

    public async Task<IResult> HandleAsync(AddBasket command, CancellationToken cancellationToken = default)
    {
        var basket = _factory.Create(command.Body.Name);
        var user = await _session.LoadAsync<User>(basket.OwnerId, cancellationToken);
        if (user is null)
        {
            _logger.LogError("User with id {UserId} not found", basket.OwnerId);
            return Results.Problem("User not found");
        }

        _session.Events.StartStream(basket.Id, new BasketCreated(basket.OwnerId, basket.Name));
        user.Baskets.Add(basket.Id);
        _session.Update(user);
        await _session.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Stream for user {userId} started with basket id {basketId}",
            basket.OwnerId.Value, basket.Id.Value);

        return Results.Ok(basket.Id.Value);
    }
}

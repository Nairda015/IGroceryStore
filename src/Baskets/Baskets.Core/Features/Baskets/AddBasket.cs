using System.Text.Json;
using EventStore.Client;
using IGroceryStore.Baskets.Core.Entities;
using IGroceryStore.Baskets.Core.Events;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using Microsoft.AspNetCore.Routing;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Shared.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace IGroceryStore.Baskets.Core.Features.Baskets;

internal record AddBasket(AddBasket.AddBasketBody Body) : IHttpCommand
{
    public record AddBasketBody(string Name);
}

public class AddBasketEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<AddBasket>("api/basket")
            .RequireAuthorization()
            .Produces<Guid>()
            .WithTags(SwaggerTags.Baskets);
}

internal class AddBasketHandler : ICommandHandler<AddBasket, IResult>
{
    private readonly EventStoreClient _client;
    private readonly ILogger<AddBasketHandler> _logger;
    private readonly IMongoCollection<User> _usersCollection;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISnowflakeService _snowflakeService;

    public AddBasketHandler(
        ILogger<AddBasketHandler> logger,
        ICurrentUserService currentUserService,
        EventStoreClient client,
        ISnowflakeService snowflakeService,
        IMongoCollection<User> usersCollection)
    {
        _logger = logger;
        _currentUserService = currentUserService;
        _client = client;
        _snowflakeService = snowflakeService;
        _usersCollection = usersCollection;
    }

    public async Task<IResult> HandleAsync(AddBasket command, CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;
        var user = await _usersCollection
            .Find(x => x.Id.Value == userId)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (user is null) //unlikely to happen
        {
            _logger.LogError("User with id {UserId} not found", userId);
            return Results.Problem("User not found");
        }

        var baskedId = _snowflakeService.GenerateId();
        var @event = new BasketCreated(userId, command.Body.Name);
        var eventData = new EventData(
            Uuid.NewUuid(),
            "basketCreated",
            JsonSerializer.SerializeToUtf8Bytes(@event));

        await _client.AppendToStreamAsync(
            baskedId.ToString(),
            StreamState.NoStream,
            new[] { eventData },
            cancellationToken: cancellationToken);

        _logger.LogInformation("Stream for user {userId} started with basket id {basketId}",
            userId, baskedId);

        return Results.Ok(baskedId);
    }
}

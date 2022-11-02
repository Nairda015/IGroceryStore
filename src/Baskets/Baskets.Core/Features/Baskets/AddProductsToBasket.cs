using System.Text.Json;
using EventStore.Client;
using IGroceryStore.Baskets.Core.Events;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Baskets.Core.Features.Baskets;

internal record AddProductsToBasket(AddProductsToBasket.AddProductsToBasketBody Body) : IHttpCommand
{
    
    public record AddProductsToBasketBody(ulong BasketId, ulong ProductId);
}

public class AddProductsToBasketEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPut<AddProductsToBasket>("/basket/{basketId}/{productId}")
            .Produces<IWriteResult>()
            .WithTags(SwaggerTags.Baskets);
}


internal class AddProductsToBasketHandler : ICommandHandler<AddProductsToBasket, IResult>
{
    private readonly EventStoreClient _client;
    private readonly ILogger<AddProductsToBasketHandler> _logger;

    public AddProductsToBasketHandler(EventStoreClient client, ILogger<AddProductsToBasketHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IResult> HandleAsync(AddProductsToBasket command, CancellationToken cancellationToken)
    {
        var (basketId, productId) = command.Body;
        
        //TODO: check if stream exist and add error handling
        
        var @event = new ProductAddedToBasket(productId);
        var eventData = new EventData(
            Uuid.NewUuid(),
            "productAddedToBasket",
            JsonSerializer.SerializeToUtf8Bytes(@event));
        
        var result = await _client.AppendToStreamAsync(basketId.ToString(),
            StreamState.StreamExists,
            new[] { eventData },
            cancellationToken: cancellationToken);
        
        return Results.Ok(result);
    }
}

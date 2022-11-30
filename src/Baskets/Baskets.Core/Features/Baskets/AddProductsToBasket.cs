using System.Text.Json;
using EventStore.Client;
using IGroceryStore.Baskets.Events;
using IGroceryStore.Shared.EndpointBuilders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Baskets.Features.Baskets;

internal record AddProductsToBasket(AddProductsToBasket.AddProductsToBasketBody Body) : IHttpCommand
{
    public record AddProductsToBasketBody(ulong BasketId, ulong ProductId);
}

public class AddProductsToBasketEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Baskets.MapPut<AddProductsToBasket, AddProductsToBasketHandler>("add-products-to-basket");
}

internal class AddProductsToBasketHandler : IHttpCommandHandler<AddProductsToBasket>
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

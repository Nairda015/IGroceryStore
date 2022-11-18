using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using IGroceryStore.Shared;
using IGroceryStore.Shops.Entities;
using IGroceryStore.Shops.Settings;
using Microsoft.Extensions.Options;

namespace IGroceryStore.Shops.Repositories;

public interface IShopsRepository
{
    Task<bool> AddChainAsync(ShopChain shop, CancellationToken cancellationToken);
    Task<ShopChain?> GetShopChainAsync(ulong id, CancellationToken cancellationToken);
    Task<bool> UpdateShopChainAsync(ShopChain shop, CancellationToken cancellationToken);
    Task<bool> ShopChainExistByNameAsync(string name, CancellationToken cancellationToken);
}

internal class ShopsRepository : IShopsRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IOptions<DynamoDbSettings> _settings;
    public ShopsRepository(IOptions<DynamoDbSettings> settings, IAmazonDynamoDB dynamoDb)
    {
        _settings = settings;
        _dynamoDb = dynamoDb;
    }
    
    public async Task<bool> AddChainAsync(ShopChain shop, CancellationToken cancellationToken)
    {
        var userAsJson = JsonSerializer.Serialize(shop);
        var itemAsDocument = Document.FromJson(userAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = Constants.TableNames.Users,
            Item = itemAsAttributes
        };

        var response = await _dynamoDb.PutItemAsync(createItemRequest, cancellationToken);
        return response.HttpStatusCode is HttpStatusCode.OK;
    }

    public async Task<ShopChain?> GetShopChainAsync(ulong id, CancellationToken cancellationToken)
    {
        var request = new GetItemRequest
        {
            TableName = Constants.TableNames.Users,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = id.ToString() } },
                { "sk", new AttributeValue { S = id.ToString() } }
            }
        };

        var response = await _dynamoDb.GetItemAsync(request, cancellationToken);
        if (response.Item.Count is 0) return null;

        var itemAsDocument = Document.FromAttributeMap(response.Item);
        return JsonSerializer.Deserialize<ShopChain>(itemAsDocument.ToJson());
    }

    public async Task<bool> UpdateShopChainAsync(ShopChain shop, CancellationToken cancellationToken)
    {
        var userAsJson = JsonSerializer.Serialize(shop);
        var itemAsDocument = Document.FromJson(userAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();

        var updateItemRequest = new PutItemRequest
        {
            TableName = Constants.TableNames.Users,
            Item = itemAsAttributes
        };

        var response = await _dynamoDb.PutItemAsync(updateItemRequest, cancellationToken);
        return response.HttpStatusCode is HttpStatusCode.OK;
    }

    public Task<bool> ShopChainExistByNameAsync(string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

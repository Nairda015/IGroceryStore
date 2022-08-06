using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using IGroceryStore.Shops.Core.Entities;
using IGroceryStore.Shops.Core.Settings;
using Microsoft.Extensions.Options;

namespace IGroceryStore.Shops.Core.Repositories;

public interface IProductsRepository
{
    Task<bool> AddAsync(Product product, CancellationToken cancellationToken);
    Task<Product?> GetAsync(ulong id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken);
}

internal class ProductsRepository : IProductsRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IOptions<DatabaseSettings> _settings;
    public ProductsRepository(IOptions<DatabaseSettings> settings, IAmazonDynamoDB dynamoDb)
    {
        _settings = settings;
        _dynamoDb = dynamoDb;
    }
    
    public async Task<bool> AddAsync(Product product, CancellationToken cancellationToken)
    {
        var productAsJson = JsonSerializer.Serialize(product);
        var itemAsDocument = Document.FromJson(productAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = _settings.Value.ProductsTable,
            Item = itemAsAttributes
        };

        var response = await _dynamoDb.PutItemAsync(createItemRequest, cancellationToken);
        return response.HttpStatusCode is HttpStatusCode.OK;
    }

    public async Task<Product?> GetAsync(ulong id, CancellationToken cancellationToken)
    {
        var request = new GetItemRequest
        {
            TableName = _settings.Value.ProductsTable,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = id.ToString() } },
                { "sk", new AttributeValue { S = id.ToString() } }
            }
        };

        var response = await _dynamoDb.GetItemAsync(request, cancellationToken);
        if (response.Item.Count == 0)
        {
            return null;
        }

        var itemAsDocument = Document.FromAttributeMap(response.Item);
        return JsonSerializer.Deserialize<Product>(itemAsDocument.ToJson());
    }

    public async Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        var productAsJson = JsonSerializer.Serialize(product);
        var itemAsDocument = Document.FromJson(productAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();

        var updateItemRequest = new PutItemRequest
        {
            TableName = _settings.Value.ProductsTable,
            Item = itemAsAttributes
        };

        var response = await _dynamoDb.PutItemAsync(updateItemRequest, cancellationToken);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}
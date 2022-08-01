using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using IGroceryStore.Shops.Core.Entities;
using IGroceryStore.Shops.Core.Settings;
using Microsoft.Extensions.Options;

namespace IGroceryStore.Shops.Core.Repositories;

public interface IUsersRepository
{
    Task<bool> AddAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(User user, CancellationToken cancellationToken);
}

internal class UsersRepository : IUsersRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IOptions<DatabaseSettings> _settings;
    public UsersRepository(IOptions<DatabaseSettings> settings, IAmazonDynamoDB dynamoDb)
    {
        _settings = settings;
        _dynamoDb = dynamoDb;
    }
    
    public async Task<bool> AddAsync(User user, CancellationToken cancellationToken)
    {
        var userAsJson = JsonSerializer.Serialize(user);
        var itemAsDocument = Document.FromJson(userAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = _settings.Value.UsersTable,
            Item = itemAsAttributes
        };

        var response = await _dynamoDb.PutItemAsync(createItemRequest, cancellationToken);
        return response.HttpStatusCode is HttpStatusCode.OK;
    }

    public async Task<User?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = new GetItemRequest
        {
            TableName = _settings.Value.UsersTable,
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
        return JsonSerializer.Deserialize<User>(itemAsDocument.ToJson());
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        var userAsJson = JsonSerializer.Serialize(user);
        var itemAsDocument = Document.FromJson(userAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();

        var updateItemRequest = new PutItemRequest
        {
            TableName = _settings.Value.UsersTable,
            Item = itemAsAttributes
        };

        var response = await _dynamoDb.PutItemAsync(updateItemRequest, cancellationToken);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}
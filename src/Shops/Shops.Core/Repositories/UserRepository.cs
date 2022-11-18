using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using IGroceryStore.Shared;
using IGroceryStore.Shops.Repositories.Contracts;
using IGroceryStore.Shops.Settings;
using Microsoft.Extensions.Options;

namespace IGroceryStore.Shops.Repositories;

public interface IUsersRepository
{
    Task<bool> AddAsync(UserDto user, CancellationToken cancellationToken);
    Task<UserDto?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(UserDto user, CancellationToken cancellationToken);
}

internal class UsersRepository : IUsersRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IOptions<DynamoDbSettings> _settings;
    public UsersRepository(IOptions<DynamoDbSettings> settings, IAmazonDynamoDB dynamoDb)
    {
        _settings = settings;
        _dynamoDb = dynamoDb;
    }
    
    public async Task<bool> AddAsync(UserDto user, CancellationToken cancellationToken)
    {
        var userAsJson = JsonSerializer.Serialize(user);
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

    public async Task<UserDto?> GetAsync(Guid id, CancellationToken cancellationToken)
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
        return JsonSerializer.Deserialize<UserDto>(itemAsDocument.ToJson());
    }

    public async Task<bool> UpdateAsync(UserDto user, CancellationToken cancellationToken)
    {
        var userAsJson = JsonSerializer.Serialize(user);
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
}

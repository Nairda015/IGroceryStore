using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using IGroceryStore.Shared.Abstraction;
using IGroceryStore.Shops.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IGroceryStore.Shops.Persistence;

internal class TableInitializer
{
    private const string ActiveTableStatus = "ACTIVE";
    private readonly AmazonDynamoDBClient _client;
    private readonly ILogger<TableInitializer> _logger;
    private readonly IOptionsMonitor<DynamoDbSettings> _settings;

    public TableInitializer(IOptionsMonitor<DynamoDbSettings> settings,
        AmazonDynamoDBClient client,
        ILogger<TableInitializer> logger)
    {
        _settings = settings;
        _client = client;
        _logger = logger;
    }

    public async Task InitializeTablesAsync()
    {
        List<Task> tasks = new()
        {
            CreateBasicTable(Constants.TableNames.Products),
            CreateBasicTable(Constants.TableNames.Users),
            CreateBasicTable(Constants.TableNames.Shops)
        };
        
        await Task.WhenAll(tasks);
    }

    private async Task<DescribeTableResponse> CreateBasicTable(string tableName)
    {
        var attributeDefinitions = new List<AttributeDefinition>
        {
            new() { AttributeName = "PK", AttributeType = "S" }, new() { AttributeName = "SK", AttributeType = "S" }
        };

        var tableKeySchema = new List<KeySchemaElement>
        {
            new() { AttributeName = "PK", KeyType = "HASH" }, new() { AttributeName = "SK", KeyType = "RANGE" }
        };

        var createTableRequest = new CreateTableRequest
        {
            TableName = tableName,
            ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 20, WriteCapacityUnits = 10 },
            AttributeDefinitions = attributeDefinitions,
            KeySchema = tableKeySchema
        };

        try
        {
            var response = await _client.CreateTableAsync(createTableRequest);
            return await WaitTillTableCreated(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Table {table} was not created", tableName);
            throw;
        }
    }

    /// <summary>
    /// Waits for successful creation of a DynamoDB table.
    /// </summary>
    /// <param name="response">A DescribeTableResponse object that can confirm successful creation of the object.</param>
    /// <returns>A DescribeTableResponse object containing information about the newly created table.</returns>
    private async Task<DescribeTableResponse> WaitTillTableCreated(CreateTableResponse response)
    {
        var request = new DescribeTableRequest { TableName = response.TableDescription.TableName };
        DescribeTableResponse resp = new DescribeTableResponse();
        var status = response.TableDescription.TableStatus;

        var initialDelay = TimeSpan.FromMilliseconds(500);
        while (status != ActiveTableStatus && MaxCreationTimeNotExceeded())
        {
            await Task.Delay(initialDelay);
            resp = await _client.DescribeTableAsync(request);
            status = resp.Table.TableStatus;
            initialDelay *= 2;
        }
        return resp;

        bool MaxCreationTimeNotExceeded()
        {
            return initialDelay < TimeSpan.FromSeconds(_settings.CurrentValue.MaxDelayForTableCreationInSeconds);
        }
    }
}

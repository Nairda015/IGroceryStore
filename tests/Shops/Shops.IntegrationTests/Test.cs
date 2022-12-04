using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using IGroceryStore.Shops.Repositories;

public sealed class LocalDynamoDbFixture : IAsyncLifetime
{
    private readonly TestcontainersContainer _testContainer;

    public LocalDynamoDbFixture()
    {
        _testContainer = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("amazon/dynamodb-local:latest")
            .WithPortBinding(8000)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8000))
            .Build();
    }

    public IShopsRepository SystemUnderTest { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await _testContainer.StartAsync();

        var credentials = new BasicAWSCredentials("FAKE", "FAKE");
        var dynamoDbConfig = new AmazonDynamoDBConfig
        {
            ServiceURL = $"http://localhost:{_testContainer.GetMappedPublicPort(8000)}",
            AuthenticationRegion = "eu-central-1"
        };
        var dynamoDb = new AmazonDynamoDBClient(credentials, dynamoDbConfig);

        SystemUnderTest = new ShopsRepository(dynamoDb);

        await CreateTablesAsync(dynamoDb);
    }

    public async Task DisposeAsync()
    {
        await _testContainer.DisposeAsync();
    }

    private async Task CreateTablesAsync(IAmazonDynamoDB dynamoDb)
    {
        var tableRequest = new CreateTableRequest
        {
            TableName = "user_trades",
            ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 1, WriteCapacityUnits = 1 },
            KeySchema = new List<KeySchemaElement>
            {
                new()
                {
                    AttributeName = "pk", KeyType = KeyType.HASH // Partition key
                },
                new()
                {
                    AttributeName = "sk", KeyType = KeyType.RANGE // Sort key
                }
            },
            AttributeDefinitions = new List<AttributeDefinition>
            {
                new() { AttributeName = "pk", AttributeType = ScalarAttributeType.S },
                new() { AttributeName = "sk", AttributeType = ScalarAttributeType.S }
            }
        };

        await dynamoDb.CreateTableAsync(tableRequest);
    }
}

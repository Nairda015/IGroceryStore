using Amazon;
using Amazon.SQS;
using IGroceryStore.API.Initializers;
using IGroceryStore.Shared.SQS;

namespace IGroceryStore.API.Configuration;

public static class AwsConfiguration
{
    public static void ConfigureSystemManager(this WebApplicationBuilder builder)
    {
        if (!builder.Environment.IsDevelopment() && !builder.Environment.IsTestEnvironment())
        {
            builder.Configuration.AddSystemsManager("/Production/IGroceryStore", TimeSpan.FromSeconds(30));
        }
        
        builder.Services.AddHostedService<SqsConsumerService>();
        builder.Services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(RegionEndpoint.EUWest2));

        builder.Services.AddSingleton<MessageDispatcher>();

        builder.Services.AddMessageHandlers();
        
        // var client = new AmazonSimpleNotificationServiceClient(RegionEndpoint.EUWest2);
        //
        // var publisher = new SnsPublisher(client);
        //
        // var customerUpdated = new CustomerUpdated
        // {
        //     Id = 1,
        //     FullName = "Nick Chapsas",
        //     LifetimeSpent = 420
        // };
        //
        // await publisher.PublishAsync("<topicARN>",
        //     customerUpdated);
    }
}

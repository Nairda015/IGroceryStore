using IGroceryStore.API.Initializers;

namespace IGroceryStore.API.Configuration;

public static class AwsConfiguration
{
    public static void ConfigureSystemManager(this WebApplicationBuilder builder)
    {
        if (!builder.Environment.IsDevelopment() && !builder.Environment.IsTestEnvironment())
        {
            builder.Configuration.AddSystemsManager("/Production/IGroceryStore", TimeSpan.FromSeconds(30));
        }
    }
}

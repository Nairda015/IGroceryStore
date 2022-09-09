using IGroceryStore.Shared.Settings;

namespace IGroceryStore.Shops.Core.Settings;

internal class DynamoDbSettings : ISettings
{
    public static string SectionName => "Shops:DynamoDb";
    public string UsersTable { get; set; } = default!;
    public string ProductsTable { get; set; } = default!;
    public string LocalServiceUrl { get; set; } = default!;
    public bool LocalMode { get; set; }
}
namespace IGroceryStore.Shops.Core.Settings;

internal class DatabaseSettings
{
    public const string KeyName = "Shops:Database";
    public string UsersTable { get; set; } = default!;
    public string ProductsTable { get; set; } = default!;
}
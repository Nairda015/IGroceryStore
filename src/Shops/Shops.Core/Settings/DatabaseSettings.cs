using IGroceryStore.Shared.Settings;

namespace IGroceryStore.Shops.Core.Settings;

internal class DatabaseSettings : ISettings
{
    public static string SectionName => "Shops:Database";
    public string UsersTable { get; set; } = default!;
    public string ProductsTable { get; set; } = default!;
}
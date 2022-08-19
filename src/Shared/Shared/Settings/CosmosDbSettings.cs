namespace IGroceryStore.Shared.Settings;

public class CosmosDbSettings : ISettings
{
    public static string SectionName => "CosmosDb";
    
    public string DbName { get; set; }
    public string Key { get; set; }
    public string Uri { get; set; }
}
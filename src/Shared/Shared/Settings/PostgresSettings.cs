namespace IGroceryStore.Shared.Settings;

public class PostgresSettings : ISettings
{
    public static string SectionName => "Postgres";
    public string ConnectionString { get; set; }
}
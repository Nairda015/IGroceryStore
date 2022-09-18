using FluentValidation;

namespace IGroceryStore.Shared.Settings;

public class PostgresSettings : SettingsBase<PostgresSettings>, ISettings
{
    public static string SectionName => "Postgres";
    public string ConnectionString { get; set; }
    public bool EnableSensitiveData { get; set; }

    public PostgresSettings()
    {
        RuleFor(x => ConnectionString).NotEmpty();
    }
}

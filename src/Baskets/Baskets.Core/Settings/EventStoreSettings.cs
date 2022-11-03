using FluentValidation;
using IGroceryStore.Shared.Settings;

namespace IGroceryStore.Baskets.Settings;

public class EventStoreSettings : SettingsBase<EventStoreSettings>, ISettings
{
    public static string SectionName => "Baskets:EventStore";
    
    public string ConnectionString { get; set; }
    
    public EventStoreSettings()
    {
        RuleFor(x => ConnectionString).NotEmpty();
    }
}

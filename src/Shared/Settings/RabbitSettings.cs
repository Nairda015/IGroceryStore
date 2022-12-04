using FluentValidation;

namespace IGroceryStore.Shared.Settings;

public class RabbitSettings : SettingsBase<RabbitSettings>, ISettings
{
    public static string SectionName => "Rabbit";
    public string Host { get; set; }
    public string VirtualHost { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public RabbitSettings()
    {
        RuleFor(x => x.Host).NotEmpty();
        RuleFor(x => x.VirtualHost).NotEmpty();
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

namespace IGroceryStore.Shared.Settings;

public record RabbitSettings : ISettings
{
    public static string SectionName => "Rabbit";
    public string Host { get; init; } = default!;
    public string VirtualHost { get; init; } = default!;
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
}
namespace IGroceryStore.Settings;

public record RabbitSettings
{
    public string Host { get; init; } = default!;
    public string VirtualHost { get; init; } = default!;
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
}
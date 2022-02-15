namespace IGroceryStore.Shared.Options;

public class JwtSettings
{
    public string Key { get; set; }
    public int ExpireSeconds { get; set; }
    public string Issuer { get; set; }
    public int ClockSkew { get; set; }
    public long TicksPerSecond = 10_000 * 1_000;
}
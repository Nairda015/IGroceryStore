namespace IGroceryStore.Shared.Services;

public sealed class DateTimeService
{
    public DateTime Now => DateTime.UtcNow;
    public DateOnly NowDateOnly => DateOnly.FromDateTime(DateTime.UtcNow);
    public TimeOnly NowTimeOnly => TimeOnly.FromDateTime(DateTime.UtcNow);
}
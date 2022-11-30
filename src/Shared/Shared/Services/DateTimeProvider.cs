namespace IGroceryStore.Shared.Services;

public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateOnly NowDateOnly { get; }
    TimeOnly NowTimeOnly { get; }
}

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.UtcNow;
    public DateOnly NowDateOnly => DateOnly.FromDateTime(DateTime.UtcNow);
    public TimeOnly NowTimeOnly => TimeOnly.FromDateTime(DateTime.UtcNow);
}

using IGroceryStore.Shared.Abstraction.Services;

namespace IGroceryStore.Services;

internal class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.UtcNow;
    public DateOnly NowDateOnly => DateOnly.FromDateTime(DateTime.UtcNow);
    public TimeOnly NowTimeOnly => TimeOnly.FromDateTime(DateTime.UtcNow);
}
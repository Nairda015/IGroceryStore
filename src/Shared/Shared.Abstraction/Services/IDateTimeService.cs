namespace IGroceryStore.Shared.Abstraction.Services;

public interface IDateTimeService
{
    public DateTime Now { get; }
    public DateOnly NowDateOnly { get; }
    public TimeOnly NowTimeOnly { get; }
}
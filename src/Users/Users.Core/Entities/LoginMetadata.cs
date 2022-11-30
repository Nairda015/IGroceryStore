namespace IGroceryStore.Users.Entities;

internal sealed class LoginMetadata
{
    public LoginMetadata(ushort accessFailedCount = 0, DateTime? lockoutEnd = null)
    {
        AccessFailedCount = accessFailedCount;
        LockoutEnd = lockoutEnd;
    }

    private const int MaxLoginTry = 5;
    public ushort AccessFailedCount { get; private set; }
    public DateTime? LockoutEnd { get; private set; }
    public bool IsLocked => LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;

    internal void ReportLoginFailure()
    {
        AccessFailedCount++;
        if (AccessFailedCount >= MaxLoginTry)
        {
            Lock();
        }
    }

    private void Lock() => LockoutEnd = DateTime.UtcNow.AddMinutes(5);
}

namespace IGroceryStore.Users.Entities;

internal sealed class SecurityMetadata
{
    public bool TwoFactorEnabled { get; set; }
    public bool EmailConfirmed { get; set; }
}

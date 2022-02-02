namespace IGroceryStore.Shared.Services;
using BCrypt.Net;

public static class HashingService
{
    
    private static string GetRandomSalt()
    {
        return BCrypt.GenerateSalt(8);
    }

    public static string HashPassword(string password)
    {
        return BCrypt.HashPassword(password, GetRandomSalt());
    }

    public static bool ValidatePassword(string password, string correctHash)
    {
        return BCrypt.Verify(password, correctHash);
    }
}
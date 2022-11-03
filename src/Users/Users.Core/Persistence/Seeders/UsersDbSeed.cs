﻿using IGroceryStore.Users.Entities;
using IGroceryStore.Users.Persistence.Contexts;

namespace IGroceryStore.Users.Persistence.Seeders;

internal static class UsersDbSeed
{
    public static async Task SeedSampleDataAsync(this UsersDbContext context)
    {
        if (context.Users.Any()) return;

        var id = new Guid(1, 3, 5, 7, 9, 2, 4, 6, 8, 0, 0);

        var user = new User(id, "Adrian", "Franczak", "adrian.franczak@gmail.com", "Password123");
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }
}

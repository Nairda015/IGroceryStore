using IGroceryStore.Baskets.Core.Entities;
using IGroceryStore.Baskets.Core.Persistence;
using IGroceryStore.Users.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Baskets.Core.Subscribers.Users;

public class AddUser : IConsumer<UserCreated>
{
    private readonly ILogger<AddUser> _logger;
    private readonly BasketDbContext _basketDbContext;

    public AddUser(ILogger<AddUser> logger, BasketDbContext basketDbContext)
    {
        _logger = logger;
        _basketDbContext = basketDbContext;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var (userId, firstName, lastName) = context.Message;
        if (await _basketDbContext.Users.AnyAsync(x => x.Id.Equals(userId))) return;

        var user = new User(userId, firstName, lastName);
        await _basketDbContext.Users.AddAsync(user);
        await _basketDbContext.SaveChangesAsync();
        _logger.LogInformation("User {UserId} added to basket database", userId);
    }
}
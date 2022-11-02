using IGroceryStore.Baskets.Core.Entities;
using IGroceryStore.Users.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace IGroceryStore.Baskets.Core.Subscribers.Users;

internal class AddUser : IConsumer<UserCreated>
{
    private readonly ILogger<AddUser> _logger;
    private readonly IMongoCollection<User> _collection;

    public AddUser(ILogger<AddUser> logger, IMongoCollection<User> collection)
    {
        _logger = logger;
        _collection = collection;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var (userId, firstName, lastName) = context.Message;
        var user = await _collection.Find(x => x.Id.Value == userId).FirstOrDefaultAsync();
        if (user is not null) return;

        user = new User
        {
            Id = userId,
            FirstName = firstName,
            LastName = lastName
        };
        
        await _collection.InsertOneAsync(user);
        _logger.LogInformation("User {UserId} added to basket database", userId);
    }
}

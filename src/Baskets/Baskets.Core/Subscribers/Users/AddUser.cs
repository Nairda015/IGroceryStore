using IGroceryStore.Baskets.Core.Entities;
using IGroceryStore.Users.Contracts.Events;
using Marten;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Baskets.Core.Subscribers.Users;

public class AddUser : IConsumer<UserCreated>
{
    private readonly ILogger<AddUser> _logger;
    private readonly IDocumentSession _session;

    public AddUser(ILogger<AddUser> logger, IDocumentSession session)
    {
        _logger = logger;
        _session = session;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var (userId, firstName, lastName) = context.Message;
        var user = await _session.LoadAsync<User>(userId, context.CancellationToken);
        if (user is not null) return;

        user = new User
        {
            Id = userId,
            FirstName = firstName,
            LastName = lastName
        };
        
        _session.Store(user);
        await _session.SaveChangesAsync();
        _logger.LogInformation("User {UserId} added to basket database", userId);
    }
}

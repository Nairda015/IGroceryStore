using IGroceryStore.Shared.Services;
using IGroceryStore.Shops.Core.Entities;
using IGroceryStore.Shops.Core.Exceptions;
using IGroceryStore.Shops.Core.Repositories;
using IGroceryStore.Users.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Shops.Core.Subscribers.Users;

public class AddUser : IConsumer<UserCreated>
{
    private readonly ILogger<AddUser> _logger;
    private readonly IUsersRepository _usersRepository;
    private readonly ISnowflakeService _snowflakeService;

    public AddUser(ILogger<AddUser> logger, IUsersRepository usersRepository, ISnowflakeService snowflakeService)
    {
        _logger = logger;
        _usersRepository = usersRepository;
        _snowflakeService = snowflakeService;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var (userId, firstName, lastName) = context.Message;

        var user = new User(userId, firstName, lastName);
        var result = await _usersRepository.AddAsync(user, context.CancellationToken);
        if (!result) throw new ShopConsumerException(
            true,
            context.CorrelationId!.Value,
            "Unable to add user to database."); 
        _logger.LogInformation("User {UserId} added to basket database", userId);
    }
}
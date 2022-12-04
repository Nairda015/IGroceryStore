using IGroceryStore.Shared.Contracts;
using IGroceryStore.Shops.Exceptions;
using IGroceryStore.Shops.Repositories;
using IGroceryStore.Shops.Repositories.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Shops.Subscribers.Users;

public class AddUser : IConsumer<UserCreated>
{
    private readonly ILogger<AddUser> _logger;
    private readonly IUsersRepository _usersRepository;

    public AddUser(ILogger<AddUser> logger, IUsersRepository usersRepository)
    {
        _logger = logger;
        _usersRepository = usersRepository;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var (userId, firstName, lastName) = context.Message;
 
        var user = new UserDto
        {
            Id = userId.ToString(),
            FirstName = firstName,
            LastName = lastName
        };
        
        var result = await _usersRepository.AddAsync(user, context.CancellationToken);
        if (!result) throw new ShopConsumerException(
            true,
            context.CorrelationId!.Value,
            "Unable to add user to database."); 
        _logger.LogInformation("User {UserId} added to basket database", userId);
    }
}

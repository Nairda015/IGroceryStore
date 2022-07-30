using DotNetCore.CAP;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Users.Contracts.Events;
using IGroceryStore.Users.Core.Exceptions;
using IGroceryStore.Users.Core.Factories;
using IGroceryStore.Users.Core.Persistence.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Users.Core.Features.Users;

public record Register(string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName) : ICommand;

public class RegisterUserEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("users/register", 
            async ([FromServices] ICommandDispatcher dispatcher, Register command) =>
        {
            await dispatcher.DispatchAsync(command);
            return Results.Accepted();
        });
    }
}

public class RegisterHandler : ICommandHandler<Register>
{
    private readonly IUserFactory _factory;
    private readonly UsersDbContext _context;
    private readonly ICapPublisher _publisher;

    public RegisterHandler(IUserFactory factory, UsersDbContext context, ICapPublisher publisher)
    {
        _factory = factory;
        _context = context;
        _publisher = publisher;
    }

    public async Task HandleAsync(Register command, CancellationToken cancellationToken = default)
    {
        var (email, password, confirmPassword, firstName, lastName) = command;
        if (password != confirmPassword) throw new PasswordDoesNotMatchException();
        
        var user = _factory.Create(Guid.NewGuid(), firstName, lastName, email, password);
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await _publisher.PublishAsync(nameof(UserCreated), new UserCreated(user.Id, firstName, lastName), cancellationToken: cancellationToken);

    }
}
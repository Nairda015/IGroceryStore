using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Users.Contracts.Events;
using IGroceryStore.Users.Core.Exceptions;
using IGroceryStore.Users.Core.Factories;
using IGroceryStore.Users.Core.Persistence.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Users.Core.Features.Users;

public record Register(string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName) : IHttpCommand;

public class RegisterUserEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<Register>("users/register").WithTags(SwaggerTags.Users);
}

public class RegisterHandler : ICommandHandler<Register, IResult>
{
    private readonly IUserFactory _factory;
    private readonly UsersDbContext _context;
    private readonly IBus _bus;

    public RegisterHandler(IUserFactory factory, UsersDbContext context, IBus bus)
    {
        _factory = factory;
        _context = context;
        _bus = bus;
    }

    public async Task<IResult> HandleAsync(Register command, CancellationToken cancellationToken = default)
    {
        var (email, password, confirmPassword, firstName, lastName) = command;
        if (password != confirmPassword) throw new PasswordDoesNotMatchException();
        
        var user = _factory.Create(Guid.NewGuid(), firstName, lastName, email, password);
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await _bus.Publish(new UserCreated(user.Id, firstName, lastName), cancellationToken: cancellationToken);
        return Results.Accepted("users/{id}", user.Id);
    }
}
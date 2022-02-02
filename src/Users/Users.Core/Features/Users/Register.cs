using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Controllers;
using IGroceryStore.Users.Core.Exceptions;
using IGroceryStore.Users.Core.Factories;
using IGroceryStore.Users.Core.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace IGroceryStore.Users.Core.Features.Users;

public record Register(string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName) : ICommand;

public class RegisterController : ApiControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public RegisterController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    [HttpPost("users/register")]
    public async Task<ActionResult> Register([FromBody] Register command)
    {
        await _dispatcher.DispatchAsync(command);
        return Ok();
    }
}

public class RegisterHandler : ICommandHandler<Register>
{
    private readonly IUserFactory _factory;
    private readonly UsersDbContext _context;

    public RegisterHandler(IUserFactory factory, UsersDbContext context)
    {
        _factory = factory;
        _context = context;
    }

    public async Task HandleAsync(Register command, CancellationToken cancellationToken = default)
    {
        var (email, password, confirmPassword, firstName, lastName) = command;
        if (password != confirmPassword) throw new PasswordDoesNotMatchException();
        
        var user = _factory.Create(Guid.NewGuid(), firstName, lastName, email, password);
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
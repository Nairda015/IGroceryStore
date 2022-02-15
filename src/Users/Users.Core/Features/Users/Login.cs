using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Controllers;
using IGroceryStore.Users.Core.Exceptions;
using IGroceryStore.Users.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Core.Features.Users;

public record Login(string Email, string Password) : ICommand<LoginResult>;
public record LoginResult(Guid UserId, string Token);

public class LoginController : ApiControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public LoginController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    [HttpPost("users/login")]
    public async Task<ActionResult<LoginResult>> Login([FromBody] Login command)
    {
        var result = await _dispatcher.DispatchAsync(command);
        return Ok(result);
    }
}

public class LoginHandler : ICommandHandler<Login, LoginResult>
{
    private readonly ITokenManager _tokenManager;
    private readonly UsersDbContext _context;

    public LoginHandler(ITokenManager tokenManager, UsersDbContext context)
    {
        _tokenManager = tokenManager;
        _context = context;
    }

    public async Task<LoginResult> HandleAsync(Login command, CancellationToken cancellationToken = default)
    {
        var (email, password) = command;
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        
        if (user is null) throw new InvalidCredentialsException();
        if (!user.Login(password)) throw new InvalidCredentialsException();
        
        var token = _tokenManager.GenerateAccessToken(user);
        return new LoginResult(user.Id, token);
    }
}
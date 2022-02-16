using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Controllers;
using IGroceryStore.Users.Core.Exceptions;
using IGroceryStore.Users.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.Services;
using IGroceryStore.Users.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Core.Features.Tokens;

public record Login(string Email, string Password);
public record LoginWithUserAgent(string Email, string Password, string UserAgent) : ICommand<LoginResult>;
public record LoginResult(Guid UserId, ReadModels.TokensReadModel Tokens);

public class LoginController : ApiControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public LoginController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    [HttpPost("tokens/login")]
    public async Task<ActionResult<LoginResult>> Login([FromBody] Login command)
    {
        var (email, password) = command;
        var agent = Request.Headers.UserAgent;
        var result = await _dispatcher.DispatchAsync(new LoginWithUserAgent(email, password, agent));
        return Ok(result);
    }
}

public class LoginHandler : ICommandHandler<LoginWithUserAgent, LoginResult>
{
    private readonly ITokenManager _tokenManager;
    private readonly UsersDbContext _context;

    public LoginHandler(ITokenManager tokenManager, UsersDbContext context)
    {
        _tokenManager = tokenManager;
        _context = context;
    }

    public async Task<LoginResult> HandleAsync(LoginWithUserAgent command, CancellationToken cancellationToken = default)
    {
        var (email, password, userAgent) = command;
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (user is null) throw new InvalidCredentialsException();
        
        if (!user.Login(password)) throw new InvalidCredentialsException();

        
        var (refreshToken, jwt) = _tokenManager.GenerateRefreshToken(user);
        user.TryRemoveOldRefreshToken(userAgent);
        user.AddRefreshToken(new RefreshToken(userAgent, refreshToken));

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        
        var tokens = new ReadModels.TokensReadModel
        {
            AccessToken = _tokenManager.GenerateAccessToken(user),
            RefreshToken = jwt
        };
        
        return new LoginResult(user.Id, tokens);
    }
}
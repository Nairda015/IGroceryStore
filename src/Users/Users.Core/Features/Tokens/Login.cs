using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Users.Core.Exceptions;
using IGroceryStore.Users.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.Services;
using IGroceryStore.Users.Core.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Core.Features.Tokens;

public record Login(string Email, string Password);
public record LoginWithUserAgent(string Email, string Password, string UserAgent) : ICommand<LoginResult>;
public record LoginResult(Guid UserId, ReadModels.TokensReadModel Tokens);

public class LoginEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("tokens/login", async (
            [FromHeader(Name = "User-Agent")] string agent,
            [FromServices] ICommandDispatcher dispatcher,
            Login command,
            CancellationToken cancellationToken) =>
        {
            var (email, password) = command;
            var result =
                await dispatcher.DispatchAsync(new LoginWithUserAgent(email, password, agent), cancellationToken);
            return Results.Ok(result);
        }).WithTags(SwaggerTags.Users);
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
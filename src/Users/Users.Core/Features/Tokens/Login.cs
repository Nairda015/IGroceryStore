using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Users.Core.Entities;
using IGroceryStore.Users.Core.Exceptions;
using IGroceryStore.Users.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.Services;
using IGroceryStore.Users.Core.ReadModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Core.Features.Tokens;

internal record LoginWithUserAgent(LoginWithUserAgent.LoginWithUserAgentBody Body) : IHttpCommand
{
    internal record LoginWithUserAgentBody(string Email,
        string Password,
        //TODO: does it work?
        [FromHeader(Name = "User-Agent")] string UserAgent);
}
internal record LoginResult(Guid UserId, TokensReadModel Tokens);


public class LoginEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<LoginWithUserAgent>("api/tokens/login")
            .Produces<LoginResult>()
            .Produces<InvalidCredentialsException>(400)
            .Produces<LoggingTriesExceededException>(400)
            .WithTags(SwaggerTags.Users);
}

internal class LoginHandler : ICommandHandler<LoginWithUserAgent, IResult>
{
    private readonly ITokenManager _tokenManager;
    private readonly UsersDbContext _context;

    public LoginHandler(ITokenManager tokenManager, UsersDbContext context)
    {
        _tokenManager = tokenManager;
        _context = context;
    }

    public async Task<IResult> HandleAsync(LoginWithUserAgent command, CancellationToken cancellationToken = default)
    {
        var (email, password, userAgent) = command.Body;
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (user is null) return Results.BadRequest(new InvalidCredentialsException());

        var loginResult = user.Login(password);
        
        if (loginResult.IsT1) return Results.BadRequest(loginResult.AsT1);
        if (!loginResult.AsT0) return Results.BadRequest(new InvalidCredentialsException());
        
        var (refreshToken, jwt) = _tokenManager.GenerateRefreshToken(user);
        user.TryRemoveOldRefreshToken(userAgent);
        user.AddRefreshToken(new ValueObjects.RefreshToken(userAgent, refreshToken));

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        var tokens = new TokensReadModel
        {
            AccessToken = _tokenManager.GenerateAccessToken(user),
            RefreshToken = jwt
        };

        var result = new LoginResult(user.Id, tokens);
        return Results.Ok(result);
    }
}

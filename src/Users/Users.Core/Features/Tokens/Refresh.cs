using System.Security.Claims;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Users.Core.Exceptions;
using IGroceryStore.Users.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Exceptions;
using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Core.ReadModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Audience = IGroceryStore.Shared.Abstraction.Constants.Tokens.Audience;

namespace IGroceryStore.Users.Core.Features.Tokens;

public record RefreshTokenCommand(Claim UserId, Claim Token) : ICommand<TokensReadModel>;

public class RefreshEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("tokens/refresh", 
            [Authorize(AuthenticationSchemes = Audience.Refresh)] async(
            [FromServices] ICommandDispatcher dispatcher,
            [FromServices] ClaimsPrincipal user) =>
        {
            var refreshToken = user.Claims.FirstOrDefault(x => x.Type == Claims.Name.RefreshToken);
            var userId = user.Claims.FirstOrDefault(x => x.Type == Claims.Name.UserId);
        
            if (refreshToken == null || userId == null) return Results.BadRequest();

            var tokens = await dispatcher.DispatchAsync(new RefreshTokenCommand(userId, refreshToken));
            return Results.Ok(tokens);
        });
    }
}

public class RefreshTokenHandler : ICommandHandler<RefreshTokenCommand, TokensReadModel>
{
    private readonly ITokenManager _tokenManager;
    private readonly UsersDbContext _context;

    public RefreshTokenHandler(ITokenManager tokenManager, UsersDbContext context)
    {
        _tokenManager = tokenManager;
        _context = context;
    }
    
    public async Task<TokensReadModel> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        var (userIdClaim, tokenClaim) = command;
        if (string.IsNullOrWhiteSpace(tokenClaim.Value)) throw new InvalidTokenException();
        
        if (!Guid.TryParse(userIdClaim.Value, out var userId)) throw new InvalidUserIdException();
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == (UserId)userId, cancellationToken);
        if (user == null) throw new UserNotFoundException(userId);

        if (!user.TokenExist(tokenClaim.Value)) throw new InvalidTokenException();
        var (refreshToken, jwt) = _tokenManager.GenerateRefreshToken(user);
        user.UpdateRefreshToken(tokenClaim.Value, refreshToken);
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new TokensReadModel
        {
            AccessToken = _tokenManager.GenerateAccessToken(user),
            RefreshToken = jwt
        };
    }
}
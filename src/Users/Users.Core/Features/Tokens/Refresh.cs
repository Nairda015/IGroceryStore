using System.Security.Claims;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Controllers;
using IGroceryStore.Users.Core.Exceptions;
using IGroceryStore.Users.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Exceptions;
using IGroceryStore.Users.Core.ReadModels;
using IGroceryStore.Users.Core.ValueObjects;

namespace IGroceryStore.Users.Core.Features.Tokens;

public record RefreshTokenCommand(Claim UserId, Claim Token) : ICommand<TokensReadModel>;

public class RefreshController : ApiControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public RefreshController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    [Authorize(AuthenticationSchemes = Shared.Abstraction.Constants.Tokens.Audience.Refresh)]
    [HttpPut("tokens/refresh", Name = "RefreshToken")]
    public async Task<ActionResult<TokensReadModel>> Refresh()
    {
        var refreshToken = User.Claims.FirstOrDefault(x => x.Type == Claims.Name.RefreshToken);
        var userId = User.Claims.FirstOrDefault(x => x.Type == Claims.Name.UserId);
        
        if (refreshToken == null || userId == null) return BadRequest();

        var tokens = await _dispatcher.DispatchAsync(new RefreshTokenCommand(userId, refreshToken));
        return Ok(tokens);
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
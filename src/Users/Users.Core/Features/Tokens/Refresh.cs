using System.Security.Claims;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Controllers;
using IGroceryStore.Users.Core.Exceptions;
using IGroceryStore.Users.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Core.Features.Tokens;

public record RefreshToken(Claim UserId, Claim Token) : ICommand<ReadModels.Tokens>;

public class RefreshController : ApiControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public RefreshController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    [Authorize(AuthenticationSchemes = Shared.Abstraction.Constants.Tokens.Audience.Refresh)]
    [HttpPut("tokens/refresh" /*,Name = Shared.Abstraction.Constants.Tokens.Audience.Refresh)*/)]
    public async Task<ActionResult<ReadModels.Tokens>> Refresh()
    {
        var refreshToken = User.Claims.FirstOrDefault(x => x.Type == Shared.Abstraction.Constants.Claims.Name.RefreshToken);
        var userId = User.Claims.FirstOrDefault(x => x.Type == Shared.Abstraction.Constants.Claims.Name.UserId);
        
        if (refreshToken == null || userId == null) return BadRequest();

        var tokens = await _dispatcher.DispatchAsync(new RefreshToken(userId, refreshToken));
        return Ok(tokens);
    }
}

public class RefreshTokenHandler : ICommandHandler<RefreshToken, ReadModels.Tokens>
{
    private readonly ITokenManager _tokenManager;
    private readonly UsersDbContext _context;

    public RefreshTokenHandler(ITokenManager tokenManager, UsersDbContext context)
    {
        _tokenManager = tokenManager;
        _context = context;
    }
    
    public async Task<ReadModels.Tokens> HandleAsync(RefreshToken command, CancellationToken cancellationToken = default)
    {
        var (userIdClaim, tokenClaim) = command;
        if (string.IsNullOrWhiteSpace(tokenClaim.Value)) throw new InvalidTokenException();
        
        if (!Guid.TryParse(userIdClaim.Value, out var userId)) throw new InvalidUserIdException();
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Value == userId, cancellationToken);
        if (user == null) throw new UserNotFoundException(userId);

        if (!user.TokenExist(tokenClaim.Value)) throw new InvalidTokenException();
        var (refreshToken, jwt) = _tokenManager.GenerateRefreshToken(user);
        user.UpdateRefreshToken(tokenClaim.Value, refreshToken);
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new ReadModels.Tokens
        {
            AccessToken = _tokenManager.GenerateAccessToken(user),
            RefreshToken = jwt
        };
    }
}
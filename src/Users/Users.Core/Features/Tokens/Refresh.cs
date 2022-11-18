using IGroceryStore.Shared.Abstraction;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Exceptions;
using IGroceryStore.Users.Persistence.Contexts;
using IGroceryStore.Users.ReadModels;
using IGroceryStore.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Audience = IGroceryStore.Shared.Abstraction.Constants.Tokens.Audience;

namespace IGroceryStore.Users.Features.Tokens;

internal record RefreshToken : IHttpCommand;

public class RefreshEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Users.MapPut<RefreshToken>("tokens/refresh")
            .RequireAuthorization(_authorizeData)
            .Produces<TokensReadModel>()
            .Produces<InvalidClaimsException>(400)
            .Produces<UserNotFoundException>(404);

    private readonly IAuthorizeData[] _authorizeData = {
        new AuthorizeAttribute { AuthenticationSchemes = Audience.Refresh }
    };
}

internal class RefreshTokenHandler : ICommandHandler<RefreshToken, IResult>
{
    private readonly ITokenManager _tokenManager;
    private readonly UsersDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RefreshTokenHandler(ITokenManager tokenManager,
        UsersDbContext context,
        ICurrentUserService currentUserService)
    {
        _tokenManager = tokenManager;
        _context = context;
        _currentUserService = currentUserService;
    }
    
    public async Task<IResult> HandleAsync(RefreshToken command, CancellationToken cancellationToken = default)
    {
        var tokenClaim = _currentUserService.Principal?.Claims.FirstOrDefault(x => x.Type is Constants.Claims.Name.RefreshToken);
        var userId = _currentUserService.UserId;

        if (userId is null) return Results.BadRequest(new InvalidClaimsException("User id not found"));
        if (tokenClaim is null) return Results.BadRequest(new InvalidClaimsException("Refresh token not found"));
        if (string.IsNullOrWhiteSpace(tokenClaim.Value)) return Results.BadRequest(new InvalidClaimsException("Refresh token not found"));
        
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == (UserId)userId, cancellationToken);
        if (user is null) return Results.NotFound(new UserNotFoundException(userId.Value));

        if (!user.TokenExist(tokenClaim.Value)) return Results.BadRequest(new InvalidClaimsException("Refresh token not found"));
        var (refreshToken, jwt) = _tokenManager.GenerateRefreshToken(user);
        user.UpdateRefreshToken(tokenClaim.Value, refreshToken);
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        
        var result = new TokensReadModel
        {
            AccessToken = _tokenManager.GenerateAccessToken(user),
            RefreshToken = jwt
        };
        return Results.Ok(result);
    }
}

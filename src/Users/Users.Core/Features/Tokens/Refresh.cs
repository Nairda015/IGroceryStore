using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Users.Core.Exceptions;
using IGroceryStore.Users.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.Services;
using Microsoft.EntityFrameworkCore;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Shared.Exceptions;
using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Core.ReadModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Audience = IGroceryStore.Shared.Abstraction.Constants.Tokens.Audience;

namespace IGroceryStore.Users.Core.Features.Tokens;

internal record RefreshTokenRequest : IHttpCommand;

public class RefreshEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPut<RefreshTokenRequest>("tokens/refresh")
            .RequireAuthorization(_authorizeData)
            .WithTags(SwaggerTags.Users);

    private readonly IAuthorizeData[] _authorizeData = {
        new AuthorizeAttribute { AuthenticationSchemes = Audience.Refresh }
    };
}

internal class RefreshTokenHandler : ICommandHandler<RefreshTokenRequest, IResult>
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
    
    public async Task<IResult> HandleAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var tokenClaim = _currentUserService.User.Claims.FirstOrDefault(x => x.Type == Claims.Name.RefreshToken);
        var userId = _currentUserService.UserId;
        
        if (tokenClaim is null) return Results.BadRequest();
        if (userId is null) throw new InvalidUserIdException();
        if (string.IsNullOrWhiteSpace(tokenClaim.Value)) throw new InvalidTokenException();
        
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == (UserId)userId, cancellationToken);
        if (user == null) throw new UserNotFoundException(userId.Value);

        if (!user.TokenExist(tokenClaim.Value)) throw new InvalidTokenException();
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
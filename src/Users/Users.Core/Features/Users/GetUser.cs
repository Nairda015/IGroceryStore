using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Queries;
using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Exceptions;
using IGroceryStore.Users.Persistence.Contexts;
using IGroceryStore.Users.ReadModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Features.Users;

internal record GetUser(Guid Id) : IHttpQuery;

public class GetUserEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet<GetUser>("api/users/{id}")
            .Produces<UserReadModel>()
            .Produces<UserNotFoundException>(404)
            .Produces(401)
            .RequireAuthorization()
            .WithName(nameof(GetUser))
            .WithTags(SwaggerTags.Users);
}


internal class GetUserHandler : IQueryHandler<GetUser, IResult>
{
    private readonly UsersDbContext _dbContext;

    public GetUserHandler(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IResult> HandleAsync(GetUser query, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == new UserId(query.Id), cancellationToken);
        if (user is null) return Results.NotFound(new UserNotFoundException(query.Id)); 
        
        var result = new UserReadModel(user.Id, user.FirstName, user.LastName, user.Email);
        return Results.Ok(result);
    }
}


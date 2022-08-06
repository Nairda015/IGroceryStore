using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Queries;
using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Core.Exceptions;
using IGroceryStore.Users.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.ReadModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Core.Features.Users;

internal record GetUser(Guid Id) : IHttpQuery;

public class GetUserEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet<GetUser>("/users/{id}").WithTags(SwaggerTags.Users);
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
        if (user is null) throw new UserNotFoundException(query.Id);
        
        var result = new UserReadModel(user.Id, user.FirstName, user.LastName, user.Email);
        return Results.Ok(result);
    }
}


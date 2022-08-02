using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Queries;
using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Core.Exceptions;
using IGroceryStore.Users.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.ReadModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Core.Features.Users;

public record GetUser(UserId Id) : IQuery<UserReadModel>;

public class GetUserEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/users/{userId:guid}", async (
            [FromServices] IQueryDispatcher dispatcher,
            Guid userId) =>
        {
            var result = await dispatcher.QueryAsync(new GetUser(userId));
            return Results.Ok(result);
        }).WithTags(SwaggerTags.Users);
    }
}


public class GetUserHandler : IQueryHandler<GetUser, UserReadModel>
{
    private readonly UsersDbContext _dbContext;

    public GetUserHandler(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserReadModel> HandleAsync(GetUser query, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);
        if (user is null) throw new UserNotFoundException(query.Id);
        
        return new UserReadModel(user.Id, user.FirstName, user.LastName, user.Email);
    }
}


using IGroceryStore.Shared.EndpointBuilders;
using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Exceptions;
using IGroceryStore.Users.Persistence.Contexts;
using IGroceryStore.Users.ReadModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Features.Users;

internal record GetUser(Guid Id) : IHttpQuery;

public class GetUserEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Users.MapGet<GetUser, GetUserHttpHandler>("{id}")
            .Produces<UserReadModel>()
            .Produces<UserNotFoundException>(404)
            .Produces(401)
            .WithName(nameof(GetUser));
}


internal class GetUserHttpHandler : IHttpQueryHandler<GetUser>
{
    private readonly UsersDbContext _dbContext;

    public GetUserHttpHandler(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IResult> HandleAsync(GetUser query, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == new UserId(query.Id), cancellationToken);
        if (user is null) return Results.NotFound(new UserNotFoundException(query.Id)); 
        
        var result = new UserReadModel(user.Id, user.FirstName, user.LastName, user.Email);
        return Results.Ok(result);
    }
}

